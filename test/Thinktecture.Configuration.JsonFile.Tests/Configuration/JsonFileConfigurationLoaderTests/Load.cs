using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.IO;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationLoaderTests
{
	public class Load : JsonFileConfigurationLoaderTestsBase
	{
		private readonly Mock<IConfigurationProvider<JToken>> _providerMock;
		private JToken _token;

		public Load()
		{
			_providerMock = new Mock<IConfigurationProvider<JToken>>(MockBehavior.Strict);
		}

		private JsonFileConfigurationLoader CreateLoader()
		{
			return new JsonFileConfigurationLoader(FileMock.Object, FilePath, ConverterMock.Object, null, null, (token, converter) =>
			{
				_token = token;
				return _providerMock.Object;
			});
		}

		private static IFileStream GetStream(string content)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

			var fileStreamMock = new Mock<IFileStream>(MockBehavior.Strict);
			fileStreamMock.Setup(s => s.Dispose()).Callback(() => stream.Dispose());
			fileStreamMock.As<IAbstraction<Stream>>().Setup(s => s.UnsafeConvert()).Returns(stream);

			return fileStreamMock.Object;
		}

		[Fact]
		public void Should_throw_if_file_throws()
		{
			FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
				.Throws<FileNotFoundException>();

			CreateLoader()
				.Invoking(l => l.Load())
				.ShouldThrow<FileNotFoundException>();
		}

		[Fact]
		public void Should_throw_if_json_is_invalid()
		{
			using (var stream = GetStream("invalid"))
			{
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
					.Returns(stream);

				CreateLoader()
					.Invoking(l => l.Load())
					.ShouldThrow<JsonReaderException>();
			}
		}

		[Fact]
		public void Should_return_provider_returned_by_factory()
		{
			using (var stream = GetStream(String.Empty))
			{
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
					.Returns(stream);

				CreateLoader().Load().Should().Be(_providerMock.Object);
			}
		}

		[Fact]
		public void Should_create_provider_with_null_as_jtoken_if_stream_is_empty()
		{
			using (var stream = GetStream(String.Empty))
			{
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
					.Returns(stream);

				CreateLoader().Load();
				_token.Should().BeNull();
			}
		}

		[Fact]
		public void Should_create_provider_with_token_of_type_null()
		{
			using (var stream = GetStream("null"))
			{
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
					.Returns(stream);
				ConverterMock.Setup(c => c.Convert<JToken>(It.IsAny<JToken>())).Returns<JToken>(token => token);

				CreateLoader().Load();
				_token.Type.Should().Be(JTokenType.Null);
			}
		}

		[Fact]
		public void Should_create_token_of_type_string()
		{
			using (var stream = GetStream("\"content\""))
			{
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
					.Returns(stream);
				ConverterMock.Setup(c => c.Convert<JToken>(It.IsAny<JToken>())).Returns<JToken>(token => token);

				CreateLoader().Load();
				_token.Value<string>().Should().Be("content");
			}
		}
	}
}