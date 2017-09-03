using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Decimal : ConvertBase
	{
		[Fact]
		public void Should_convert_decimal_property_if_value_is_not_null()
		{
			SetupCreateFromString<decimal>("42.1", new ConversionResult(42.1m));

			RoundtripConvert<TestConfiguration<decimal>>("P1", "42.1")
				.P1.ShouldBeEquivalentTo(42.1m);
		}

		[Fact]
		public void Should_convert_decimal_property()
		{
			SetupCreateFromString<decimal>("42", new ConversionResult(42m));

			RoundtripConvert<TestConfiguration<decimal>>("P1", "42")
				.P1.ShouldBeEquivalentTo(42m);
		}

		[Fact]
		public void Should_convert_nullable_decimal_property_if_value_is_not_null()
		{
			SetupCreateFromString<decimal?>("42", new ConversionResult(42m));

			RoundtripConvert<TestConfiguration<decimal?>>("P1", "42")
				.P1.ShouldBeEquivalentTo(42m);
		}

		[Fact]
		public void Should_convert_nullable_decimal_property_if_value_is_null()
		{
			RoundtripConvert<TestConfiguration<decimal?>>("P1", null)
				.P1.Should().BeNull();
		}

		[Fact]
		public void Should_convert_nullable_decimal_property_if_value_is_empty_string()
		{
			SetupCreateFromString<decimal?>(String.Empty, new ConversionResult(null));

			RoundtripConvert<TestConfiguration<decimal?>>("P1", String.Empty)
				.P1.Should().BeNull();
		}
	}
}
