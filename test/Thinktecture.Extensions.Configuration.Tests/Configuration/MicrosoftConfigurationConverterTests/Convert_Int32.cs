using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Int32 : ConvertBase
	{
		[Fact]
		public void Should_convert_int_property_if_value_is_not_null()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(int), "42")).Returns(42);

			var result = RoundtripConvert<TestConfiguration<int>>("P1", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<int>() {P1 = 42});
		}

		[Fact]
		public void Should_throw_if_int_property_is_floating_point_number()
		{
			Action action = () => RoundtripConvert<TestConfiguration<int>>("P1", "42.1");
			action.ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void Should_convert_int_property_if_value_is_null()
		{
			var result = RoundtripConvert<TestConfiguration<int>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<int>() {P1 = 0});
		}

		[Fact]
		public void Should_convert_int_property_if_value_is_empty_string()
		{
			Action action = () => RoundtripConvert<TestConfiguration<int>>("P1", String.Empty);
			action.ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void Should_throw_if_int_property_is_not_a_number()
		{
			Action action = () => RoundtripConvert<TestConfiguration<int>>("P1", "value");
			action.ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void Should_convert_nullable_int_property_if_value_is_not_null()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(int), "42")).Returns(42);

			var result = RoundtripConvert<TestConfiguration<int?>>("P1", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<int?>() {P1 = 42});
		}

		[Fact]
		public void Should_convert_nullable_int_property_if_value_is_null()
		{
			var result = RoundtripConvert<TestConfiguration<int?>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<int?>() {P1 = null});
		}

		[Fact]
		public void Should_convert_nullable_int_property_if_value_is_empty_string()
		{
			var result = RoundtripConvert<TestConfiguration<int?>>("P1", String.Empty);
			result.ShouldBeEquivalentTo(new TestConfiguration<int?>() {P1 = null});
		}
	}
}