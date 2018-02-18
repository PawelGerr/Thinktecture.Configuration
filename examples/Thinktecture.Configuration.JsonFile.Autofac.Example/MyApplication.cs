using System;
using Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class MyApplication : IMyApplication
	{
		private readonly IMyComponent _component;
		private readonly IMyApplicationConfiguration _config;

		public MyApplication(IMyComponent component, IMyApplicationConfiguration config)
		{
			_component = component;
			_config = config;
		}

		public void PrintConfiguration()
		{
			Console.WriteLine(Environment.NewLine + "Application configuration: " + _config.ApplicationValue);
			_component.PrintConfiguration();
		}
	}
}
