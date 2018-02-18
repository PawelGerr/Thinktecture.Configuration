using System;
using Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class MyComponent : IMyComponent
	{
		private readonly IMyComponentConfiguration _config;

		public MyComponent(IMyComponentConfiguration config)
		{
			_config = config ?? throw new ArgumentNullException(nameof(config));
		}

		public void PrintConfiguration()
		{
			Console.WriteLine("Component configuration: " + _config.ComponentValue);
			Console.WriteLine("Component configuration -> other configuration: " + _config.OtherConfiguration?.OtherComponentValue);
		}
	}
}
