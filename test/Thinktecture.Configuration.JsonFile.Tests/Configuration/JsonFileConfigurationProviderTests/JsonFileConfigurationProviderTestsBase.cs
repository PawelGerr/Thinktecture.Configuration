using System.Linq;
using Moq;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration.JsonFileConfigurationProviderTests
{
	public abstract class JsonFileConfigurationProviderTestsBase
	{
		protected readonly Mock<IJsonTokenConverter> ConverterMock;

		protected JsonFileConfigurationProviderTestsBase()
		{
			ConverterMock = new Mock<IJsonTokenConverter>(MockBehavior.Strict);
		}

		protected JsonFileConfigurationProvider CreateProvider(params object[] objects)
		{
			var tokens = objects == null ? new JToken[] { null } : objects.Select(o => o == null ? null : JToken.FromObject(o)).ToArray();
			return new JsonFileConfigurationProvider(tokens, ConverterMock.Object);
		}

		protected JToken[] GetTokens(params object[] objects)
		{
			return objects.Select(o => JToken.FromObject(o)).ToArray();
		}
	}
}
