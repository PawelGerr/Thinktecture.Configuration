using System;
using System.Globalization;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Double : ConvertBase
	{
		[Fact]
		public void Should_convert_double_property_with_decimal_places()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(double), "42.1")).Returns(42.1);

			var result = RoundtripConvert<TestConfiguration<double>>("P1", "42.1");
			result.ShouldBeEquivalentTo(new TestConfiguration<double>() {P1 = 42.1});
		}

		[Fact]
		public void Should_convert_double_property()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(double), "42")).Returns(42);

			var result = RoundtripConvert<TestConfiguration<double>>("P1", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<double>() {P1 = 42});
		}

		[Fact]
		public void Should_convert_nullable_double_property_if_value_is_not_null()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(double), "42")).Returns(42d);

			var result = RoundtripConvert<TestConfiguration<double?>>("P1", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<double?>() {P1 = 42});
		}

		[Fact]
		public void Should_convert_nullable_double_property_if_value_is_null()
		{
			var result = RoundtripConvert<TestConfiguration<double?>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<double?>() {P1 = null});
		}

		[Fact]
		public void Should_convert_nullable_double_property_if_value_is_empty_string()
		{
			var result = RoundtripConvert<TestConfiguration<double?>>("P1", String.Empty);
			result.ShouldBeEquivalentTo(new TestConfiguration<double?>() {P1 = null});
		}
	}
}