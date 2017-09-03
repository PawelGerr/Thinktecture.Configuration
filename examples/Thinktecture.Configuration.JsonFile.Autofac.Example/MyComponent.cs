using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example
{
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
