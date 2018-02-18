using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_Struct : ConvertBase
	{
		public Convert_Struct(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void Should_throw_if_configuration_is_null()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Action action = () => Converter.Convert<TestStruct>(null);
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_configuration_is_null_using_non_generic_overload()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Action action = () => Converter.Convert(null, typeof(TestStruct));
			action.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Should_return_default_value_if_creation_of_config_failed()
		{
			var config = GetConfig(new object());
			Converter.Convert<TestStruct>(config).Should().Be(new TestStruct());
		}

		[Fact]
		public void Should_convert_empty_string_to_default_value()
		{
			SetupCreateFromString<TestStruct>(String.Empty, ConversionResult.Invalid);

			var result = RoundtripConvert<TestConfiguration<TestStruct>>("P1", String.Empty);
			result.P1.Should().Be(new TestStruct());
		}

		[Fact]
		public void Should_convert_inner_complex_property()
		{
			SetupCreate<TestConfiguration<TestStruct>>(new ConversionResult(new TestConfiguration<TestStruct>()));

			var result = RoundtripConvert(new TestConfiguration<TestConfiguration<TestStruct>>()
			{
				P1 = new TestConfiguration<TestStruct>() { P1 = new TestStruct() { Value = "struct value" } }
			});

			result.Should().BeEquivalentTo(new TestConfiguration<TestConfiguration<TestStruct>>()
			{
				P1 = new TestConfiguration<TestStruct>() { P1 = new TestStruct() { Value = "struct value" } }
			});
		}
	}
}
