using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Extensions.Configuration.Example.Configuration
{
	public class MyInnerConfiguration : IMyInnerConfiguration
	{
		public string InnerValue { get; set; }
		public MyDependency Dependency { get; }

		public MyInnerConfiguration(MyDependency dependency)
		{
			Dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
		}
	}
}