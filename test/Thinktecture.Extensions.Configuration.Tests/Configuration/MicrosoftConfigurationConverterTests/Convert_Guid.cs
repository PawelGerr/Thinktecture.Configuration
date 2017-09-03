using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Guid : ConvertBase
	{
		[Fact]
		public void Should_set_property_when_creator_returns_valid_result()
		{
			SetupCreateFromString("31033C8F-FE32-41BF-934B-3C23DC85F28F", new Guid("31033C8F-FE32-41BF-934B-3C23DC85F28F"));

			RoundtripConvert<TestConfiguration<Guid>>("P1", "31033C8F-FE32-41BF-934B-3C23DC85F28F")
				.P1.Should().Be(new Guid("31033C8F-FE32-41BF-934B-3C23DC85F28F"));
		}
	}
}
