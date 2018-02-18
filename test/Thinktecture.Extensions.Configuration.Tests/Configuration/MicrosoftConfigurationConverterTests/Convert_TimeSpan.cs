using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_TimeSpan : ConvertBase
	{
		public Convert_TimeSpan(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void Should_set_property_when_creator_returns_valid_result()
		{
			SetupCreateFromString("12:34:56", new TimeSpan(0, 12, 34, 56));

			RoundtripConvert<TestConfiguration<TimeSpan>>("P1", "12:34:56")
				.P1.Should().Be(new TimeSpan(0, 12, 34, 56));
		}
	}
}
