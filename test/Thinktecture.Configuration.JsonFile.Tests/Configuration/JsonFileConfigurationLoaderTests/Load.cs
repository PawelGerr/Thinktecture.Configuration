﻿using System;
using System.IO;
using System.Linq;
using System.Text;
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
		private readonly Mock<IConfigurationProvider<JToken, JToken>> _providerMock;
		private JToken[] _tokens;

		public Load()
		{
			_providerMock = new Mock<IConfigurationProvider<JToken, JToken>>(MockBehavior.Strict);
		}

		private JsonFileConfigurationLoader CreateLoader(params string[] filePaths)
		{
			return new JsonFileConfigurationLoader(FileMock.Object, ConverterMock.Object, filePaths, null, null, (token, converter) =>
			{
				_tokens = token;
				return _providerMock.Object;
			});
		}

		private IFileStream GetStream(string content)
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

			CreateLoader(FilePath)
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

				CreateLoader(FilePath)
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

				CreateLoader(FilePath).Load().Should().Be(_providerMock.Object);
			}
		}

		[Fact]
		public void Should_create_provider_with_null_as_jtoken_if_stream_is_empty()
		{
			using (var stream = GetStream(String.Empty))
			{
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
						.Returns(stream);

				CreateLoader(FilePath).Load();
				_tokens.Should().HaveCount(1)
						.And.Contain((JToken)null);
			}
		}

		[Fact]
		public void Should_create_provider_with_token_of_type_null()
		{
			using (var stream = GetStream("null"))
			{
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
						.Returns(stream);

				CreateLoader(FilePath).Load();
				_tokens.Should().HaveCount(1)
						.And.Subject.First().Type.Should().Be(JTokenType.Null);
			}
		}

		[Fact]
		public void Should_create_token_of_type_string()
		{
			using (var stream = GetStream("\"content\""))
			{
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
						.Returns(stream);

				CreateLoader(FilePath).Load();
				_tokens.First().Value<string>().Should().Be("content");
			}
		}

		[Fact]
		public void Should_create_provider_with_2_token_if_2_filepaths_are_provided()
		{
			using (var stream = GetStream("\"content\""))
			using (var stream2 = GetStream("\"content2\""))
			{
				var nextStream = stream;
				FileMock.Setup(f => f.Open(It.IsAny<string>(), It.IsAny<FileMode>(), It.IsAny<FileAccess>(), It.IsAny<FileShare>()))
						.Returns(() =>
						{
							var currentStream = nextStream;
							// ReSharper disable once AccessToDisposedClosure
							nextStream = stream2;
							return currentStream;
						});

				CreateLoader(FilePath, OverrideFilePath).Load();
				_tokens.Should().HaveCount(2);
				_tokens[0].Value<string>().Should().Be("content");
				_tokens[1].Value<string>().Should().Be("content2");
			}
		}
	}
}
