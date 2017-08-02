using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Creates and populates types using <see cref="IConfiguration"/>.
	/// </summary>
	public class MicrosoftConfigurationConverter : IMicrosoftConfigurationConverter
	{
		private readonly ILogger<MicrosoftConfigurationConverter> _logger;
		private readonly IInstanceCreator _instanceCreator;

		/// <summary>
		/// Initializes new instance of <see cref="MicrosoftConfigurationConverter"/>.
		/// </summary>
		/// <param name="logger">Logger</param>
		/// <param name="instanceCreator">Creates new instances of provided type.</param>
		public MicrosoftConfigurationConverter(ILogger<MicrosoftConfigurationConverter> logger, IInstanceCreator instanceCreator)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
		}

		/// <summary>
		/// Creates a new instance of type <typeparamref name="T"/> and populates it with values from <paramref name="configuration"/>.
		/// </summary>
		/// <typeparam name="T">Type an instance to create of.</typeparam>
		/// <param name="configuration">Provides values to populate an instance of type <typeparamref name="T"/>.</param>
		/// <returns>A new instance of type <typeparamref name="T"/>.</returns>
		public T Convert<T>(IConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			var result = Convert(configuration, typeof(T));

			try
			{
				return (T) result;
			}
			catch (NullReferenceException ex)
			{
				if (result == null)
					throw new ConfigurationSerializationException($"The provided configuration could not be deserialized because the deserialization returned null but the type ${typeof(T).FullName} is not nullable.", ex);

				throw new ConfigurationSerializationException($"Error during deserialization of the type {typeof(T).FullName}.", ex);
			}
		}

		/// <summary>
		/// Creates a new instance of provided <paramref name="type"/> and populates it with values from <paramref name="configuration"/>.
		/// </summary>
		/// <param name="configuration">Provides values to populate an instance of provided <paramref name="type"/>.</param>
		/// <param name="type">Type an instance to create of.</param>
		/// <returns>A new instance of provided <paramref name="type"/>.</returns>
		public object Convert(IConfiguration configuration, Type type)
		{
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			var result = CreateAndPopulate(type, configuration, ConversionInstance.Empty);

			if (result.IsValid && result.Value != null)
				return result.Value;

			return _instanceCreator.CreateDefaultValue(type);
		}

		private IConversionResult CreateAndPopulate(Type type, IConfiguration config, IConversionInstance instance)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var hasChildConfigs = config.GetChildren().Any();
			var configValue = (config as IConfigurationSection)?.Value;

			if (!hasChildConfigs && configValue == null)
				return new ConversionResult(null);

			if (type.IsArray)
			{
				var elementType = type.GetElementType();

				if (instance.IsCreated)
				{
					var currentArraySize = (instance.Value as Array)?.Length;

					if (currentArraySize > 0)
					{
						_logger.LogWarning(@"One of the parent configuration objects has a property of type {type} that contains {size} elements before deserializion. 
This array along with its elements are going to be discarded. Please make check that no memory leaks occur. Configuration path: {path}", $"{elementType.Name}[]", currentArraySize, (config as IConfigurationSection)?.Path);
					}
				}

				return CreateAndPopulateArray(elementType, config);
			}

			if (IsComplexType(type, config))
				return ConvertComplexType(type, config, instance);

			if (configValue != null)
				return ConvertFromString(type, configValue);

			if (instance.IsCreated)
				return new ConversionResult(instance.Value);

			return ConversionResult.Invalid;
		}

		private bool IsComplexType(Type type, IConfiguration config)
		{
			var typeInfo = type.GetTypeInfo();

			var isSimpleType = typeInfo.IsPrimitive
			                   || type == typeof(string)
			                   || type == typeof(decimal)
			                   || type == typeof(DateTime)
			                   || type == typeof(TimeSpan)
			                   || type == typeof(Guid)
			                   || (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));

			if (isSimpleType)
				return false;

			if (config.GetChildren().Any())
				return true;

			return (config as IConfigurationSection)?.Value != null;
		}

		private IConversionResult ConvertComplexType(Type type, IConfiguration config, IConversionInstance instance)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			if (!instance.IsCreated || instance.Value == null)
			{
				var result = _instanceCreator.Create(type);

				if (result == null)
					throw new ConfigurationSerializationException($"Instance creator returned null when trying to create an instance of type {type.FullName}");

				if (!result.IsValid)
					return result;

				instance = new ConversionInstance(result.Value);
			}

			if (instance.Value == null)
				return new ConversionResult(instance.Value);

			if (TryGetDictionaryTypes(type, out var dictionaryTypes))
			{
				PopulateDictionary(dictionaryTypes.KeyType, dictionaryTypes.ValueType, config, instance);
			}
			else if (TryGetCollectionElementType(type, out var elementType))
			{
				PopulateCollection(elementType, config, instance);
			}

			Populate(config, instance);

			return new ConversionResult(instance.Value);
		}

		private void Populate(IConfiguration config, IConversionInstance instance)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));
			if (instance.Value == null)
				throw new ArgumentNullException(nameof(instance), "Instance cannot be null when populating.");

			var properties = GetProperties(instance.Value.GetType());
			var keyLookup = new HashSet<string>(config.GetChildren().Select(c => c.Key), StringComparer.OrdinalIgnoreCase);

			foreach (var property in properties)
			{
				if (keyLookup.Contains(property.Name))
				{
					var childSection = config.GetSection(property.Name);
					SetProperty(property, childSection, instance);
				}
			}
		}

		private void SetProperty(PropertyInfo property, IConfiguration config, IConversionInstance instance)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));
			if (instance.Value == null)
				throw new ArgumentNullException(nameof(instance), "Instance cannot be null when setting a property.");

			if (!HasPublicGetter(property))
				return;

			var propertyValue = property.GetValue(instance.Value);
			var hasPublicSetter = property.SetMethod != null && property.SetMethod.IsPublic;

			if (!hasPublicSetter && propertyValue == null)
				return;

			var newValueResult = CreateAndPopulate(property.PropertyType, config, new ConversionInstance(propertyValue));

			if (newValueResult.IsValid && hasPublicSetter && !ReferenceEquals(propertyValue, newValueResult.Value))
			{
				if (newValueResult.Value == null)
				{
					var typeInfo = property.PropertyType.GetTypeInfo();

					if (typeInfo.IsValueType && !(typeInfo.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
						throw new ConfigurationSerializationException($"Cannot assign null to non-nullable type {property.PropertyType.FullName}. Path: {(config as IConfigurationSection)?.Path}");
				}
				property.SetValue(instance.Value, newValueResult.Value);
			}
		}

		private static bool HasPublicGetter(PropertyInfo property)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));

			return property.GetMethod != null
			       && property.GetMethod.IsPublic
			       && property.GetMethod.GetParameters().Length == 0;
		}

		private void PopulateCollection(Type elementType, IConfiguration config, IConversionInstance instance)
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));
			if (instance.Value == null)
				throw new ArgumentNullException(nameof(instance), "Collection cannot be null when populating.");
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			var arrayResult = CreateAndPopulateArray(elementType, config);

			if (arrayResult.IsValid)
				PopulateCollection(instance.Value, elementType, (Array) arrayResult.Value);
		}

		private void PopulateCollection(object collection, Type elementType, Array children)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			var collectionType = collection.GetType();
			var addMethod = FindMethod(collectionType, "Add", elementType);

			if (addMethod == null)
			{
				_logger.LogWarning("Could not populate a collection of type {type} because no method with signature {signature} has been found.", $"{collectionType.Name}<{elementType.Name}>", $"Add({elementType.Name} value)");
				return;
			}

			foreach (var child in children)
			{
				try
				{
					addMethod.Invoke(collection, new[] {child});
				}
				catch (Exception ex)
				{
					_logger.LogError(0, ex, "Add");
				}
			}
		}

		private MethodInfo FindMethod(Type type, string name, params Type[] paramTypes)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (name == null)
				throw new ArgumentNullException(nameof(name));
			if (paramTypes == null)
				throw new ArgumentNullException(nameof(paramTypes));

			if (type == typeof(object))
				return null;

			while (type != typeof(object))
			{
				var typeInfo = type.GetTypeInfo();

				if (typeInfo.IsInterface)
					return null;

				var methodInfo = typeInfo.DeclaredMethods
					.FirstOrDefault(m =>
					{
						if (!StringComparer.OrdinalIgnoreCase.Equals(m.Name, name))
							return false;

						var parameters = m.GetParameters();

						if (parameters.Length != paramTypes.Length)
							return false;

						for (int i = 0; i < parameters.Length; i++)
						{
							if (parameters[i].ParameterType != paramTypes[i])
								return false;
						}

						return true;
					});

				if (methodInfo != null)
					return methodInfo;

				type = typeInfo.BaseType;
			}

			return null;
		}

		private bool TryGetCollectionElementType(Type type, out Type elementType)
		{
			var collectionType = FindGenericImplementedType(typeof(IList<>), type)
			                     ?? FindGenericImplementedType(typeof(ICollection<>), type)
			                     ?? FindGenericImplementedType(typeof(IReadOnlyList<>), type)
			                     ?? FindGenericImplementedType(typeof(IReadOnlyCollection<>), type)
			                     ?? FindGenericImplementedType(typeof(IEnumerable<>), type);

			elementType = collectionType?.GetTypeInfo().GenericTypeArguments[0];

			return elementType != null;
		}

		private bool TryGetDictionaryTypes(Type type, out (Type KeyType, Type ValueType) types)
		{
			var dictionaryInterface = FindGenericImplementedType(typeof(IDictionary<,>), type)
			                          ?? FindGenericImplementedType(typeof(IReadOnlyDictionary<,>), type);

			if (dictionaryInterface != null)
			{
				var typeInfo = type.GetTypeInfo();
				types = (typeInfo.GenericTypeArguments[0], typeInfo.GenericTypeArguments[1]);
				return true;
			}

			types = (null, null);
			return false;
		}

		private void PopulateDictionary(Type keyType, Type valueType, IConfiguration config, IConversionInstance instance)
		{
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));
			if (instance.Value == null)
				throw new ArgumentNullException(nameof(instance), "Dictionary cannot be null when populating.");
			if (keyType == null)
				throw new ArgumentNullException(nameof(keyType));
			if (valueType == null)
				throw new ArgumentNullException(nameof(valueType));
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			var dictionary = instance.Value;
			var dictionaryType = dictionary.GetType();
			var addMethod = FindMethod(dictionaryType, "Add", keyType, valueType);

			if (addMethod == null)
			{
				_logger.LogWarning("Could not populate a dictionary of type {type} because no method with signature {signature} has been found.", $"{dictionaryType.Name}<{keyType.Name},{valueType.Name}>", $"Add({keyType.Name} key, {valueType.Name} value)");
				return;
			}

			foreach (var child in config.GetChildren())
			{
				var itemResult = CreateAndPopulate(valueType, child, ConversionInstance.Empty);

				if (itemResult.IsValid)
				{
					var keyResult = ConvertFromString(keyType, child.Key);
					if (keyResult.IsValid)
						addMethod.Invoke(dictionary, new[] {keyResult.Value, itemResult.Value});
				}
			}
		}

		private IConversionResult CreateAndPopulateArray(Type elementType, IConfiguration config)
		{
			if (elementType == null)
				throw new ArgumentNullException(nameof(elementType));
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			var children = config.GetChildren()
				.Select(c =>
				{
					int index;
					if (c.Key == null || !Int32.TryParse(c.Key, out index))
					{
						_logger.LogWarning("The index of the collection of type {type} is not an integer. Key: {key}, path: {path}", elementType.FullName, c.Key, c.Path);
						return null;
					}

					return new {Index = index, Configuration = c};
				})
				.Where(i => i != null)
				.ToArray();

			if (children.Length == 0)
			{
				if ((config as IConfigurationSection)?.Value == null)
					return new ConversionResult(null);
			}

			var array = _instanceCreator.CreateArray(elementType, children.Length == 0 ? 0 : children.Max(c => c.Index) + 1);

			foreach (var child in children)
			{
				var itemResult = CreateAndPopulate(elementType, child.Configuration, ConversionInstance.Empty);
				if (itemResult.IsValid)
					array.SetValue(itemResult.Value, child.Index);
			}

			return new ConversionResult(array);
		}

		private IConversionResult ConvertFromString(Type type, string value)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (type == typeof(object))
				return new ConversionResult(value);

			return _instanceCreator.Create(type, value);
		}

		private Type FindGenericImplementedType(Type expected, Type actual)
		{
			var actualTypeInfo = actual.GetTypeInfo();

			if (actualTypeInfo.IsGenericType && actual.GetGenericTypeDefinition() == expected)
				return actual;

			foreach (var interfaceType in actualTypeInfo.ImplementedInterfaces)
			{
				if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == expected)
					return interfaceType;
			}

			return null;
		}

		private List<PropertyInfo> GetProperties(Type type)
		{
			var props = new List<PropertyInfo>();

			while (type != typeof(object))
			{
				var typeInfo = type.GetTypeInfo();
				props.AddRange(typeInfo.DeclaredProperties);

				type = typeInfo.BaseType;
			}

			return props;
		}
	}
}