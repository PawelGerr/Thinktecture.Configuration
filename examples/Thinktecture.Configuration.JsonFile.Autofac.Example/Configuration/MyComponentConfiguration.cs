using System;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration
{
	// ReSharper disable UnusedAutoPropertyAccessor.Global
	// ReSharper disable once ClassNeverInstantiated.Global
	public class MyComponentConfiguration : IMyComponentConfiguration
	{
		public TimeSpan ComponentValue { get; set; }
		public IMyComponentOtherConfiguration OtherConfiguration { get; set; }
		public ClassWithTypeConverter ClassWithTypeConverter { get; set; }
		public StructConfiguration StructConfiguration { get; set; }
	}
}
