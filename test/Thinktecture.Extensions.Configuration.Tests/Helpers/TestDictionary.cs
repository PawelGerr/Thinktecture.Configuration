using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Helpers
{
	public class TestDictionary<TKey, TValue, TProperty> : Dictionary<TKey, TValue>
	{
		public TProperty Property { get; set; }
	}
}