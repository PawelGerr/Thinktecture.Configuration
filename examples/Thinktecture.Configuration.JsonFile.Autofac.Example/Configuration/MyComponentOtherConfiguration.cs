using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thinktecture.IO;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration
{
	public class MyComponentOtherConfiguration : IMyComponentOtherConfiguration
	{
		public decimal OtherComponentValue { get; set; }

		public MyComponentOtherConfiguration(IFile file)
		{
			Console.WriteLine("Has IFile been injected: " + (file != null ? "yes" : "no"));
		}
	}
}
