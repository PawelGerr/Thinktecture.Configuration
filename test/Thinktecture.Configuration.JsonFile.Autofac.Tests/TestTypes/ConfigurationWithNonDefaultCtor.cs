using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.TestTypes
{
	public class ConfigurationWithNonDefaultCtor
		: IConfigurationWithNonDefaultCtor
	{
		public SimpleDependency SimpleDependency { get; }

		public ConfigurationWithNonDefaultCtor(SimpleDependency simpleDependency)
		{
			SimpleDependency = simpleDependency;
		}
	}
}
