using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Extensions.Configuration.Example.Configuration
{
	public interface IMyConfiguration
	{
		int Value { get; }
		bool Boolean { get; }
		IMyInnerConfiguration InnerConfiguration { get; }
		int[] IntArray { get; }
		ICollection<int> IntCollection { get; }
		IDictionary<string, int> StringIntDictionary { get; }
	}
}
