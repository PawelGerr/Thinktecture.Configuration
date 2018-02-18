using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationLoaderTests
{
	[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
	public class Ctor : JsonFileConfigurationLoaderTestsBase
	{
		[Fact]
		public void Should_throw_if_file_is_null()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Action ctor = () => new JsonFileConfigurationLoader(null, ConverterMock.Object, new[] { FilePath });

			ctor.Invoking(c => c()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_filepaths_are_null()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Action ctor = () => new JsonFileConfigurationLoader(FileMock.Object, ConverterMock.Object, null);

			ctor.Invoking(c => c()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_one_of_the_filepaths_is_null()
		{
			Action ctor = () => new JsonFileConfigurationLoader(FileMock.Object, ConverterMock.Object, new string[] { null });

			ctor.Invoking(c => c()).ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Should_throw_if_one_of_the_filepaths_is_empty()
		{
			Action ctor = () => new JsonFileConfigurationLoader(FileMock.Object, ConverterMock.Object, new[] { " " });

			ctor.Invoking(c => c()).ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Should_throw_if_converter_is_null()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Action ctor = () => new JsonFileConfigurationLoader(FileMock.Object, null, new[] { FilePath });

			ctor.Invoking(c => c()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_initialize_new_instance_without_calling_any_members()
		{
			new JsonFileConfigurationLoader(FileMock.Object, ConverterMock.Object, new[] { FilePath });
		}

		[Fact]
		public void Should_initialize_new_instance_without_calling_any_members_getting_2_filepaths()
		{
			new JsonFileConfigurationLoader(FileMock.Object, ConverterMock.Object, new[] { FilePath, OverrideFilePath });
		}
	}
}
