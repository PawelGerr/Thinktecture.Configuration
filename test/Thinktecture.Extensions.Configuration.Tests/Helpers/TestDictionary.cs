using System.Collections.Generic;

namespace Thinktecture.Helpers
{
	public class TestDictionary<TKey, TValue, TProperty> : Dictionary<TKey, TValue>
	{
		public TProperty Property { get; set; }
	}
}
