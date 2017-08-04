using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Shared logic for implementations of <see cref="IInstanceCreator"/>.
	/// </summary>
	public abstract class InstanceCreator : IInstanceCreator
	{
		/// <summary>
		/// Culture to be used for parsing of values.
		/// </summary>
		protected readonly CultureInfo Culture;

		/// <summary>
		/// Initializes new instance of type <see cref="InstanceCreator"/>.
		/// </summary>
		/// <param name="culture">Culture to be used during conversion.</param>
		protected InstanceCreator(CultureInfo culture)
		{
			Culture = culture ?? throw new ArgumentNullException(nameof(culture));
		}

		/// <inheritdoc />
		public abstract IConversionResult Create(Type type);

		/// <inheritdoc />
		public IConversionResult Create(Type type, string value)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			try
			{
				if(type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					if(string.IsNullOrWhiteSpace(value))
						return new ConversionResult(null);

					type = Nullable.GetUnderlyingType(type);
				}

				var instance = TypeDescriptor.GetConverter(type).ConvertFromString(null, Culture, value);

				return new ConversionResult(instance);
			}
			catch (Exception ex)
			{
				throw new ConfigurationSerializationException($"Could not create type {type.FullName} from string \"{value}\"", ex);
			}
		}

		/// <inheritdoc />
		public Array CreateArray(Type elementType, int length)
		{
			if (elementType == null)
				throw new ArgumentNullException(nameof(elementType));

			return Array.CreateInstance(elementType, length);
		}
	}
}