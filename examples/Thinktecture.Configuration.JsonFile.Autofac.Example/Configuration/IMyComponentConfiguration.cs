using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration
{
	public interface IMyComponentConfiguration
	{
		TimeSpan ComponentValue { get; }
		IMyComponentOtherConfiguration OtherConfiguration { get; }
		ClassWithTypeConverter ClassWithTypeConverter { get; }
		StructConfiguration StructConfiguration { get; }
	}
}
