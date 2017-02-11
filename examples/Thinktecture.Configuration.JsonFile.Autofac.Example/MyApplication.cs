using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example
{
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