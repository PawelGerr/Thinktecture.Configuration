using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_Decimal : ConvertBase
	{
		public Convert_Decimal(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void Should_convert_decimal_property_if_value_is_not_null()
		{
			SetupCreateFromString("42.1", 42.1m);

			RoundtripConvert<TestConfiguration<decimal>>("P1", "42.1")
				.P1.Should().Be(42.1m);
		}

		[Fact]
		public void Should_convert_decimal_property()
		{
			SetupCreateFromString("42", 42m);

			RoundtripConvert<TestConfiguration<decimal>>("P1", "42")
				.P1.Should().Be(42m);
		}

		[Fact]
		public void Should_convert_nullable_decimal_property_if_value_is_not_null()
		{
			SetupCreateFromString("42", 42m);

			RoundtripConvert<TestConfiguration<decimal?>>("P1", "42")
				.P1.Should().Be(42m);
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
