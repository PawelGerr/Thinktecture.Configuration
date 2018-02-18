using System;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_Boolean : ConvertBase
	{
		[Fact]
		public void Should_throw_if_value_is_null_but_the_type_is_not_nullable()
		{
			Action action = () => RoundtripConvert<TestConfiguration<bool>>("P1", null);
			action.Should().Throw<ConfigurationSerializationException>().WithMessage("Cannot assign null to non-nullable type System.Boolean. Path: P1");
		}

		[Fact]
		public void Should_set_property_when_creator_returns_valid_result()
		{
			SetupCreateFromString("true", true);

			RoundtripConvert<TestConfiguration<bool>>("P1", "true")
				.P1.Should().BeTrue();
		}

		[Fact]
		public void Should_not_set_property_when_creator_returns_invalid_result()
		{
			SetupCreateFromString<bool>("true", ConversionResult.Invalid);

			RoundtripConvert<TestConfiguration<bool>>("P1", "true")
				.P1.Should().BeFalse();
		}

		[Fact]
		public void Should_throw_when_creator_throws()
		{
			SetupCreateFromString<bool>("true", s => throw new Exception("Error!"));

			Action action = () => RoundtripConvert<TestConfiguration<bool>>("P1", "true");
			action.Should().Throw<Exception>().WithMessage("Error!");
		}

		[Fact]
		public void Should_convert_value_using_instance_creator_when_value_is_empty_string()
		{
			SetupCreateFromString(String.Empty, true);

			RoundtripConvert<TestConfiguration<bool>>("P1", String.Empty)
				.P1.Should().BeTrue();

			InstanceCreatorMock.Verify(c => c.Create(typeof(bool), String.Empty), Times.Once);
		}

		[Fact]
		public void Should_convert_nullable_boolean_using_instance_creator_if_value_is_not_null()
		{
			SetupCreateFromString("true", true);

			RoundtripConvert<TestConfiguration<bool?>>("P1", "true")
				.P1.Should().BeTrue();
		}

		[Fact]
		public void Should_convert_nullable_boolean_when_value_is_null()
		{
			RoundtripConvert<TestConfiguration<bool?>>("P1", null)
				.P1.Should().BeNull();
		}
	}
}
