using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Information about the provided input.
	/// </summary>
	public class ConversionInstance : IConversionInstance
	{
		/// <summary>
		/// Input that has to be created yet.
		/// </summary>
		public static readonly ConversionInstance Empty = new ConversionInstance(false, null);

		/// <inheritdoc />
		public bool IsCreated { get; }

		private readonly object _value;

		/// <inheritdoc />
		public object Value => IsCreated ? _value : throw new InvalidOperationException("Value is not created yet.");

		/// <summary>
		/// Initializes new instance of <see cref="ConversionInstance"/>
		/// </summary>
		/// <param name="value">Value to be used for population.</param>
		public ConversionInstance(object value)
			: this(true, value)
		{
		}

		private ConversionInstance(bool isCreated, object value)
		{
			IsCreated = isCreated;
			_value = value;
		}
	}
}
