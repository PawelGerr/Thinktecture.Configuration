using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_TimeSpan : ConvertBase
	{
		[Fact]
		public void Should_set_property_when_creator_returns_valid_result()
		{
			SetupCreateFromString("12:34:56", new TimeSpan(0, 12, 34, 56));

			RoundtripConvert<TestConfiguration<TimeSpan>>("P1", "12:34:56")
				.P1.Should().Be(new TimeSpan(0, 12, 34, 56));
		}
	}
}
