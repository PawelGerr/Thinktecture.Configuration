using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_Double : ConvertBase
	{
		[Fact]
		public void Should_convert_when_value_is_not_empty()
		{
			SetupCreateFromString("42.1", 42.1d);

			RoundtripConvert<TestConfiguration<double>>("P1", "42.1")
				.P1.Should().Be(42.1d);
		}

		[Fact]
		public void Should_convert_double_property()
		{
			SetupCreateFromString("42", 42d);

			RoundtripConvert<TestConfiguration<double>>("P1", "42")
				.P1.Should().Be(42d);
		}

		[Fact]
		public void Should_convert_nullable_double_property_if_value_is_not_null()
		{
			SetupCreateFromString("42", 42d);

			RoundtripConvert<TestConfiguration<double?>>("P1", "42")
				.P1.Should().Be(42d);
		}

		[Fact]
		public void Should_convert_nullable_double_property_if_value_is_null()
		{
			RoundtripConvert<TestConfiguration<double?>>("P1", null)
				.P1.Should().BeNull();
		}

		[Fact]
		public void Should_convert_nullable_double_property_if_value_is_empty_string()
		{
			SetupCreateFromString<double?>(String.Empty, new ConversionResult(null));

			RoundtripConvert<TestConfiguration<double?>>("P1", String.Empty)
				.P1.Should().BeNull();
		}
	}
}
