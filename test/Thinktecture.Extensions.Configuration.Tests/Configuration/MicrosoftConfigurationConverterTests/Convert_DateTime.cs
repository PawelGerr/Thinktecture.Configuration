using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_DateTime : ConvertBase
	{
		[Fact]
		public void Should_set_property_when_creator_returns_valid_result()
		{
			SetupCreateFromString("2017-08-01T12:34:56.789", new DateTime(2017, 08, 01, 12, 34, 56, 789, DateTimeKind.Unspecified));

			RoundtripConvert<TestConfiguration<DateTime>>("P1", "2017-08-01T12:34:56.789")
				.P1.Should().Be(new DateTime(2017, 08, 01, 12, 34, 56, 789, DateTimeKind.Unspecified));
		}
	}
}