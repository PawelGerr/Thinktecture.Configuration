using System;
using System.ComponentModel;
using System.Globalization;
using JetBrains.Annotations;

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
		// ReSharper disable once MemberCanBePrivate.Global
		protected readonly CultureInfo Culture;

		/// <summary>
		/// Initializes new instance of type <see cref="InstanceCreator"/>.
		/// </summary>
		/// <param name="culture">Culture to be used during conversion.</param>
		protected InstanceCreator([NotNull] CultureInfo culture)
		{
			Culture = culture ?? throw new ArgumentNullException(nameof(culture));
		}

		/// <inheritdoc />
		public abstract IConversionResult Create(Type type);

		/// <inheritdoc />
		public virtual IConversionResult Create(Type type, string value)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			try
			{
				var converter = TypeDescriptor.GetConverter(type);

				if (!converter.CanConvertFrom(typeof(string)))
					return ConversionResult.Invalid;

				// ReSharper disable once AssignNullToNotNullAttribute
				var instance = converter.ConvertFromString(null, Culture, value);

				return new ConversionResult(instance);
			}
			catch (Exception ex)
			{
				throw new ConfigurationSerializationException($"Could not create type {type.FullName} from string \"{value}\"", ex);
			}
		}

		/// <inheritdoc />
		public virtual Array CreateArray(Type elementType, int length)
		{
			if (elementType == null)
				throw new ArgumentNullException(nameof(elementType));

			return Array.CreateInstance(elementType, length);
		}
	}
}
