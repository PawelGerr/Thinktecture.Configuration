using System.Collections.Generic;

namespace Thinktecture.Extensions.Configuration.Example.Configuration
{
	// ReSharper disable UnusedMember.Global
	public interface IMyConfiguration
	{
		int Value { get; }
		bool Boolean { get; }
		IMyInnerConfiguration InnerConfiguration { get; }
		int[] IntArray { get; }
		ICollection<int> IntCollection { get; }
		IDictionary<string, int> StringIntDictionary { get; }
		ClassWithTypeConverter ClassWithTypeConverter { get; }
		StructConfiguration StructConfiguration { get; }
	}
}
