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
	public interface IConversionInstance
	{
		/// <summary>
		/// Indication whether the value is created and ready to be used or has to be created.
		/// </summary>
		bool IsCreated { get; }

		/// <summary>
		/// Value to be used for population.
		/// </summary>
		object Value { get; }
	}
}