using Moq;
using Thinktecture.IO;

namespace Thinktecture.Configuration.JsonFileConfigurationLoaderTests
{
	public abstract class JsonFileConfigurationLoaderTestsBase
	{
		protected readonly Mock<IFile> FileMock;
		protected readonly Mock<IJsonTokenConverter> ConverterMock;
		protected readonly string FilePath;
		protected readonly string OverrideFilePath;

		protected JsonFileConfigurationLoaderTestsBase()
		{
			FileMock = new Mock<IFile>(MockBehavior.Strict);
			ConverterMock = new Mock<IJsonTokenConverter>(MockBehavior.Strict);
			FilePath = "/file.json";
			OverrideFilePath = "/override.json";
		}
	}
}
