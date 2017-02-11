using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration
{
	public class MyApplicationConfiguration : IMyApplicationConfiguration
	{
		public string ApplicationValue { get; set; }
	}
}