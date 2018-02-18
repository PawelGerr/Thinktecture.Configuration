using System;
using Thinktecture.IO;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration
{
	// ReSharper disable UnusedAutoPropertyAccessor.Global
	// ReSharper disable once ClassNeverInstantiated.Global
	public class MyComponentOtherConfiguration : IMyComponentOtherConfiguration
	{
		public decimal OtherComponentValue { get; set; }

		public MyComponentOtherConfiguration(IFile file)
		{
			Console.WriteLine("Has IFile been injected: " + (file != null ? "yes" : "no"));
		}
	}
}
