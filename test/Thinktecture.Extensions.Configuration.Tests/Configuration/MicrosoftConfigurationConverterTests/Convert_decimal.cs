using System;
using System.Globalization;
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
			InstanceCreatorMock.Setup(c => c.Create(typeof(decimal), "42.1")).Returns(42.1m);

			var result = RoundtripConvert<TestConfiguration<decimal>>("P1", "42.1");
			result.ShouldBeEquivalentTo(new TestConfiguration<decimal>() {P1 = 42.1m});
		}

		[Fact]
		public void Should_convert_decimal_property()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(decimal), "42")).Returns(42m);

			var result = RoundtripConvert<TestConfiguration<decimal>>("P1", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<decimal>() {P1 = 42});
		}

		[Fact]
		public void Should_convert_nullable_decimal_property_if_value_is_not_null()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(decimal), "42")).Returns(42m);

			var result = RoundtripConvert<TestConfiguration<decimal?>>("P1", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<decimal?>() {P1 = 42});
		}

		[Fact]
		public void Should_convert_nullable_decimal_property_if_value_is_null()
		{
			var result = RoundtripConvert<TestConfiguration<decimal?>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<decimal?>() {P1 = null});
		}

		[Fact]
		public void Should_convert_nullable_decimal_property_if_value_is_empty_string()
		{
			var result = RoundtripConvert<TestConfiguration<decimal?>>("P1", String.Empty);
			result.ShouldBeEquivalentTo(new TestConfiguration<decimal?>() {P1 = null});
		}
	}
}