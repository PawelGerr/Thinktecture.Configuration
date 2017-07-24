using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.Extensions.Configuration.Example.Configuration
{
	public interface IMyInnerConfiguration
	{
		string InnerValue { get; }
	}
}