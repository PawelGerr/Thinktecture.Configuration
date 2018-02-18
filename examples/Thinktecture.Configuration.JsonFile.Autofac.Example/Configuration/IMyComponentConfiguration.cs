using System;

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
