using System.Collections.Generic;

namespace Thinktecture.Helpers
{
	public class TestList<TItem, TProperty> : List<TItem>
	{
		public TProperty Property { get; set; }
	}
}