using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Type to resolve by the <see cref="AutofacJsonTokenConverter"/>.
	/// </summary>
	public class AutofacJsonTokenConverterType
	{
		/// <summary>
		/// Type to be resolved.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// Initializes new instance <see cref="AutofacJsonTokenConverter"/>.
		/// </summary>
		/// <param name="type">Type to resolve.</param>
		public AutofacJsonTokenConverterType(Type type)
		{
			Type = type ?? throw new ArgumentNullException(nameof(type));
		}
	}
}
