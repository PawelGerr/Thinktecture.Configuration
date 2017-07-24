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

			return (T) Convert(configuration, typeof(T));
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

			return CreateAndPopulate(type, configuration);
		}

		private object CreateAndPopulate(Type type, IConfiguration config, object instance = null)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			if (type.IsArray)
			{
				var elementType = type.GetElementType();
				var currentArraySize = (instance as Array)?.Length;

				if (currentArraySize > 0)
				{
					_logger.LogWarning(@"One of the parent configuration objects has a property of type {type} that contains {size} elements before deserializion. 
This array along with its elements are going to be discarded. Please make check that no memory leaks occur. Configuration path: {path}", $"{elementType.Name}[]", currentArraySize, (config as IConfigurationSection)?.Path);
				}

				return CreateAndPopulateArray(elementType, config);
			}

			if (IsComplexType(type, config))
				return ConvertComplexType(type, config, instance);

			var configValue = (config as IConfigurationSection)?.Value;

			if (configValue != null)
				return ConvertValue(type, configValue);

			return instance;
		}

		private bool IsComplexType(Type type, IConfiguration config)
		{
			var typeInfo = type.GetTypeInfo();

			return type != typeof(string)
			       && type != typeof(decimal)
			       && (!typeInfo.IsGenericType || type.GetGenericTypeDefinition() != typeof(Nullable<>))
			       && (config.GetChildren().Any() || !typeInfo.IsPrimitive);
		}

		private object ConvertComplexType(Type type, IConfiguration config, object instance)
		{
			if (instance == null)
				instance = _instanceCreator.Create(type);

			if (instance == null)
			{
				_logger.LogDebug("Instance creator returned null when trying to create an instance of type {type}", type.FullName);
				return null;
			}

			var dictionaryTypes = GetDictionaryTypes(type);
			if (dictionaryTypes.DictionaryType != null)
			{
				PopulateDictionary(instance, dictionaryTypes.DictionaryType, dictionaryTypes.KeyType, dictionaryTypes.ValueType, config);
			}
			else
			{
				var collectionTypes = GetCollectionType(type);
				if (collectionTypes.CollectionType != null)
					PopulateCollection(instance, collectionTypes.CollectionType, collectionTypes.ElementType, config);
			}

			BindProperties(config, instance);

			return instance;
		}

		private void BindProperties(IConfiguration config, object instance)
		{
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			var properties = GetProperties(instance.GetType());
			var keyLookup = new HashSet<string>(config.GetChildren().Select(c => c.Key), StringComparer.OrdinalIgnoreCase);

			foreach (var property in properties)
			{
				if (keyLookup.Contains(property.Name))
				{
					var childSection = config.GetSection(property.Name);
					BindProperty(property, childSection, instance);
				}
			}
		}

		private void BindProperty(PropertyInfo property, IConfiguration config, object instance)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));
			if (config == null)
				throw new ArgumentNullException(nameof(config));
			if (instance == null)
				throw new ArgumentNullException(nameof(instance));

			if (!HasPublicGetter(property))
				return;

			var propertyValue = property.GetValue(instance);
			var hasPublicSetter = property.SetMethod != null && property.SetMethod.IsPublic;

			if (!hasPublicSetter && propertyValue == null)
				return;

			var newPropertyValue = CreateAndPopulate(property.PropertyType, config, propertyValue);

			if (hasPublicSetter && !ReferenceEquals(propertyValue, newPropertyValue))
				property.SetValue(instance, newPropertyValue);
		}

		private static bool HasPublicGetter(PropertyInfo property)
		{
			if (property == null)
				throw new ArgumentNullException(nameof(property));

			return property.GetMethod != null
			       && property.GetMethod.IsPublic
			       && property.GetMethod.GetParameters().Length == 0;
		}

		private void PopulateCollection(object collection, Type collectionType, Type elementType, IConfiguration config)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			var array = CreateAndPopulateArray(elementType, config);
			PopulateCollection(collection, collectionType, elementType, array);
		}

		private void PopulateCollection(object collection, Type collectionType, Type elementType, Array children)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));
			if (children == null)
				throw new ArgumentNullException(nameof(children));

			var methods = collectionType.GetTypeInfo().GetDeclaredMethods("Add")
				.Concat(collection.GetType().GetTypeInfo().GetDeclaredMethods("Add"));

			var addMethod = methods.FirstOrDefault(info =>
			{
				var parameters = info.GetParameters();
				return parameters.Length == 1 && parameters[0].ParameterType == elementType;
			});

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

		private (Type CollectionType, Type ElementType) GetCollectionType(Type type)
		{
			var collectionType = FindGenericImplementedType(typeof(IList<>), type)
			                     ?? FindGenericImplementedType(typeof(ICollection<>), type)
			                     ?? FindGenericImplementedType(typeof(IReadOnlyList<>), type)
			                     ?? FindGenericImplementedType(typeof(IReadOnlyCollection<>), type)
			                     ?? FindGenericImplementedType(typeof(IEnumerable<>), type);

			var typeInfo = type.GetTypeInfo();
			var elementType = typeInfo.IsGenericType ? typeInfo.GenericTypeArguments[0] : typeof(object);

			return (collectionType, elementType);
		}

		private (Type DictionaryType, Type KeyType, Type ValueType) GetDictionaryTypes(Type type)
		{
			var dictionaryInterface = FindGenericImplementedType(typeof(IDictionary<,>), type)
			                          ?? FindGenericImplementedType(typeof(IReadOnlyDictionary<,>), type);

			if (dictionaryInterface != null)
			{
				var typeInfo = type.GetTypeInfo();
				return (dictionaryInterface, typeInfo.GenericTypeArguments[0], typeInfo.GenericTypeArguments[1]);
			}

			return (null, null, null);
		}

		private void PopulateDictionary(object dictionary, Type dictionaryType, Type keyType, Type valueType, IConfiguration config)
		{
			if (dictionary == null)
				throw new ArgumentNullException(nameof(dictionary));
			if (dictionaryType == null)
				throw new ArgumentNullException(nameof(dictionaryType));
			if (keyType == null)
				throw new ArgumentNullException(nameof(keyType));
			if (valueType == null)
				throw new ArgumentNullException(nameof(valueType));
			if (config == null)
				throw new ArgumentNullException(nameof(config));

			var addMethod = dictionaryType.GetTypeInfo().GetDeclaredMethods("Add")
				.Concat(dictionary.GetType().GetTypeInfo().GetDeclaredMethods("Add"))
				.FirstOrDefault(info =>
				{
					var parameters = info.GetParameters();
					return parameters.Length == 2 && parameters[0].ParameterType == keyType && parameters[1].ParameterType == valueType;
				});

			if (addMethod == null)
			{
				_logger.LogWarning("Could not populate a dictionary of type {type} because no method with signature {signature} has been found.", $"{dictionaryType.Name}<{keyType.Name},{valueType.Name}>", $"Add({keyType.Name} key, {valueType.Name} value)");
				return;
			}

			foreach (var child in config.GetChildren())
			{
				var item = CreateAndPopulate(valueType, child);

				if (item != null)
				{
					var key = child.Key;
					addMethod.Invoke(dictionary, new[] {key, item});
				}
			}
		}

		private Array CreateAndPopulateArray(Type elementType, IConfiguration config)
		{
			var children = config.GetChildren()
				.Select(c =>
				{
					int index;
					if (!Int32.TryParse(c.Key, out index))
					{
						_logger.LogWarning("The index of an array of type {type}[] is not an integer. Key: {key}, path: {path}", elementType.Name, c.Key, c.Path);
						return null;
					}

					return new {Index = index, Configuration = c};
				})
				.Where(i => i != null)
				.ToArray();

			var array = _instanceCreator.CreateArray(elementType, children.Length == 0 ? 0 : children.Max(c => c.Index) + 1);

			foreach (var child in children)
			{
				try
				{
					var item = CreateAndPopulate(elementType, child.Configuration);
					if (item != null)
						array.SetValue(item, child.Index);
				}
				catch (Exception ex)
				{
					_logger.LogWarning(0, ex, "An array element of type {type} could be converted.", elementType.Name);
				}
			}

			return array;
		}

		private object ConvertValue(Type type, string value)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			if (value == null || type == typeof(object))
				return value;

			if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				return string.IsNullOrEmpty(value) ? null : ConvertValue(Nullable.GetUnderlyingType(type), value);
			}

			try
			{
				return _instanceCreator.Create(type, value);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Cannot convert provided value to type {type.FullName}.", ex);
			}
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