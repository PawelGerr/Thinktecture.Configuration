using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_DateTime : ConvertBase
	{
		public Convert_DateTime(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void Should_set_property_when_creator_returns_valid_result()
		{
			SetupCreateFromString("2017-08-01T12:34:56.789", new DateTime(2017, 08, 01, 12, 34, 56, 789, DateTimeKind.Unspecified));

			RoundtripConvert<TestConfiguration<DateTime>>("P1", "2017-08-01T12:34:56.789")
				.P1.Should().Be(new DateTime(2017, 08, 01, 12, 34, 56, 789, DateTimeKind.Unspecified));
		}
	}
}
