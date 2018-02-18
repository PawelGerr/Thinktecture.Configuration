using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Class : ConvertBase
	{
		[Fact]
		public void Should_throw_if_configuration_is_null()
		{
			Action action = () => Converter.Convert<TestConfiguration<int>>(null);
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_configuration_is_null_using_non_generic_overload()
		{
			Action action = () => Converter.Convert(null, typeof(TestConfiguration<int>));
			action.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_return_default_value_if_creation_of_config_failed()
		{
			SetupCreate<TestConfiguration<int>>(ConversionResult.Invalid);

			var config = GetConfig(new object());
			Converter.Convert<int>(config).Should().Be(0);
		}

		[Fact]
		public void Should_convert_null_to_null()
		{
			SetupCreateFromString<TestConfiguration<int>>(String.Empty, ConversionResult.Invalid);

			var result = RoundtripConvert<TestConfiguration<TestConfiguration<int>>>(dictionary => dictionary["P1"] = null);
			result.P1.Should().BeNull();
		}

		[Fact]
		public void Should_convert_empty_string_to_empty_object()
		{
			SetupCreate<TestConfiguration<int>>(new ConversionResult(new TestConfiguration<int>()));
			SetupCreateFromString<TestConfiguration<int>>(String.Empty, ConversionResult.Invalid);

			var result = RoundtripConvert<TestConfiguration<TestConfiguration<int>>>(dictionary => dictionary["P1"] = String.Empty);
			result.P1.ShouldBeEquivalentTo(new TestConfiguration<int>());
		}

		[Fact]
		public void Should_convert_inner_complex_property()
		{
			SetupCreate<TestConfiguration<decimal>>(new ConversionResult(new TestConfiguration<decimal>()));
			SetupCreateFromString("42", 42m);

			var result = RoundtripConvert(new TestConfiguration<TestConfiguration<decimal>>()
			{
				P1 = new TestConfiguration<decimal>() { P1 = 42 }
			});

			result.ShouldBeEquivalentTo(new TestConfiguration<TestConfiguration<decimal>>()
			{
				P1 = new TestConfiguration<decimal>() { P1 = 42 }
			});
		}
	}
}
