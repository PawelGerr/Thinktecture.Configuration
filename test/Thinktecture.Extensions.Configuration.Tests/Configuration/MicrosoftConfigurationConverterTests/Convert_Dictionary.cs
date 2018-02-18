using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_Dictionary : ConvertBase
	{
		public Convert_Dictionary()
		{
			InstanceCreatorMock.Setup(c => c.Create(It.IsAny<Type>()))
			                   .Returns<Type>(type => new ConversionResult(new Dictionary<string, int>()));
		}

		[Fact]
		public void Should_convert_null_to_null()
		{
			RoundtripConvert<TestConfiguration<Dictionary<string, int>>>("P1", null)
				.P1.Should().BeNull();
		}

		[Fact]
		public void Should_convert_empty_string_to_empty_collection()
		{
			SetupCreateFromString<Dictionary<string, int>>(String.Empty, ConversionResult.Invalid);

			RoundtripConvert<TestConfiguration<Dictionary<string, int>>>("P1", String.Empty)
				.P1.Should().BeEmpty();
		}

		[Fact]
		public void Should_convert_dictionary_with_one_value()
		{
			SetupCreateFromString("42", 42);

			RoundtripConvert<TestConfiguration<Dictionary<string, int>>>("P1:foo", "42")
				.P1.Should().BeEquivalentTo(new Dictionary<string, int>() { ["foo"] = 42 });
		}

		[Fact]
		public void Should_convert_idictionary_with_one_value()
		{
			SetupCreateFromString("42", 42);

			RoundtripConvert<TestConfiguration<IDictionary<string, int>>>("P1:foo", "42")
				.P1.Should().BeEquivalentTo(new Dictionary<string, int>() { ["foo"] = 42 });
		}

		[Fact]
		public void Should_convert_ireadonlydictionary_with_one_value()
		{
			SetupCreateFromString("42", 42);

			RoundtripConvert<TestConfiguration<IReadOnlyDictionary<string, int>>>("P1:foo", "42")
				.P1.Should().BeEquivalentTo(new Dictionary<string, int>() { ["foo"] = 42 });
		}
	}
}
