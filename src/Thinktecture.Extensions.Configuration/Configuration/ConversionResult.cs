using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// The result of the conversion.
	/// </summary>
	public class ConversionResult : IConversionResult
	{
		/// <summary>
		/// Represents an invalid conversion result.
		/// </summary>
		public static readonly ConversionResult Invalid = new ConversionResult(false, null);

		/// <inheritdoc />
		public bool IsValid { get; }

		private readonly object _value;

		/// <inheritdoc />
		public object Value => IsValid ? _value : throw new InvalidOperationException("Value is not valid");

		/// <summary>
		/// Initializes new instance of <see cref="ConversionResult"/>.
		/// </summary>
		/// <param name="value">Valid value to be used by the <see cref="IMicrosoftConfigurationConverter"/>.</param>
		public ConversionResult(object value)
			: this(true, value)
		{
		}

		private ConversionResult(bool isValid, object value)
		{
			IsValid = isValid;
			_value = value;
		}
	}
}