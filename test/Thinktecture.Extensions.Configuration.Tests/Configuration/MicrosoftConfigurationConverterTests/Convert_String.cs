using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_String : ConvertBase
	{
		[Fact]
		public void Should_convert_non_empty_string()
		{
			RoundtripConvert<TestConfiguration<string>>("P1", "value")
				.P1.Should().Be("value");
		}

		[Fact]
		public void Should_convert_empty_string()
		{
			RoundtripConvert<TestConfiguration<string>>("P1", String.Empty)
				.P1.Should().BeEmpty();
		}

		[Fact]
		public void Should_convert_null_string()
		{
			RoundtripConvert<TestConfiguration<string>>("P1", null)
				.P1.Should().BeNull();
		}
	}
}
