using System;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Int32 : ConvertBase
	{
		[Fact]
		public void Should_convert_int_property_if_value_is_not_null()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<int>>("P1", "42")
				.P1.ShouldBeEquivalentTo(42);
		}

		[Fact]
		public void Should_throw_if_creator_throws()
		{
			SetupCreateFromString<int>("42", v => throw new Exception("Error!"));

			Action action = () => RoundtripConvert<TestConfiguration<int>>("P1", "42");
			action.ShouldThrow<Exception>().WithMessage("Error!");
		}

		[Fact]
		public void Should_throw_if_value_is_null_but_the_type_is_not_nullable()
		{
			Action action = () => RoundtripConvert<TestConfiguration<int>>("P1", null);
			action.ShouldThrow<ConfigurationSerializationException>().WithMessage("Cannot assign null to non-nullable type System.Int32. Path: P1");
		}

		[Fact]
		public void Should_convert_value_using_instance_creator_when_value_is_empty_string()
		{
			SetupCreateFromString<int>(String.Empty, new ConversionResult(42));

			RoundtripConvert<TestConfiguration<int>>("P1", String.Empty)
				.P1.Should().Be(42);
		}

		[Fact]
		public void Should_convert_value_using_instance_creator_when_value_is_an_invalid_integer()
		{
			SetupCreateFromString<int>("not-an-int", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<int>>("P1", "not-an-int")
				.P1.Should().Be(42);
		}

		[Fact]
		public void Should_convert_nullable_int_property_if_value_is_not_null()
		{
			SetupCreateFromString<int?>("42", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<int?>>("P1", "42")
				.P1.Should().Be(42);
		}

		[Fact]
		public void Should_convert_nullable_int_property_if_value_is_null()
		{
			RoundtripConvert<TestConfiguration<int?>>("P1", null)
				.P1.Should().BeNull();
		}

		[Fact]
		public void Should_convert_nullable_int_property_if_value_is_empty_string()
		{
			SetupCreateFromString<int?>(String.Empty, new ConversionResult(null));

			RoundtripConvert<TestConfiguration<int?>>("P1", String.Empty)
				.P1.Should().BeNull();
		}
	}
}