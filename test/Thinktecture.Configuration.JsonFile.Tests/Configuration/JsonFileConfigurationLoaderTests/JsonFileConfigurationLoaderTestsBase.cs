using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json.Linq;
using Thinktecture.IO;

namespace Thinktecture.Configuration.JsonFileConfigurationLoaderTests
{
	public abstract class JsonFileConfigurationLoaderTestsBase
	{
		protected readonly Mock<IFile> FileMock;
		protected readonly Mock<IJsonTokenConverter> ConverterMock;
		protected readonly string FilePath;

		protected JsonFileConfigurationLoaderTestsBase()
		{
			FileMock = new Mock<IFile>(MockBehavior.Strict);
			ConverterMock = new Mock<IJsonTokenConverter>(MockBehavior.Strict);
			FilePath = "/file.json";
		}

		protected JsonFileConfigurationProvider CreateProvider(object o)
		{
			var token = o == null ? null : JToken.FromObject(o);
			return new JsonFileConfigurationProvider(token, ConverterMock.Object);
		}
	}
}