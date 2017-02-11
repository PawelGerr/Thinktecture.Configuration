using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace Thinktecture.Configuration.JsonFileConfigurationProviderTests
{
	public abstract class JsonFileConfigurationProviderTestsBase
	{
		protected readonly Mock<IJsonTokenConverter> ConverterMock;

		protected JsonFileConfigurationProviderTestsBase()
		{
			ConverterMock = new Mock<IJsonTokenConverter>(MockBehavior.Strict);
		}
	}
}