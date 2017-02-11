using System;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationLoaderTests
{
	public class Ctor : JsonFileConfigurationLoaderTestsBase
	{
		[Fact]
		public void Should_throw_if_file_is_null()
		{
			Action ctor = () => new JsonFileConfigurationLoader(null, FilePath, ConverterMock.Object);

			ctor.Invoking(c => c()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_filepath_is_null()
		{
			Action ctor = () => new JsonFileConfigurationLoader(FileMock.Object, null, ConverterMock.Object);

			ctor.Invoking(c => c()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_converter_is_null()
		{
			Action ctor = () => new JsonFileConfigurationLoader(FileMock.Object, FilePath, null);

			ctor.Invoking(c => c()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_initialize_new_instance_without_calling_any_members()
		{
			new JsonFileConfigurationLoader(FileMock.Object, FilePath, ConverterMock.Object);
		}
	}
}