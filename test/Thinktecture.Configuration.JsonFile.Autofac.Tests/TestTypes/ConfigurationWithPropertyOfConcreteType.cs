using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.TestTypes
{
	public class ConfigurationWithPropertyOfConcreteType : IConfigurationWithPropertyOfConcreteType
	{
		public ConfigurationWithDefaultCtor InnerConfiguration { get; set; }
	}
}
