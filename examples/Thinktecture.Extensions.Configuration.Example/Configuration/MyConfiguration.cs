using System.Collections.Generic;

namespace Thinktecture.Extensions.Configuration.Example.Configuration
{
	public class MyConfiguration : IMyConfiguration
	{
		public int Value { get; set; }
		public bool Boolean{ get; set; }
		public IMyInnerConfiguration InnerConfiguration { get; set; }
		public int[] IntArray { get; set; }
		public ICollection<int> IntCollection { get; set; }
		public IDictionary<string, int> StringIntDictionary { get; set; }
	}
}