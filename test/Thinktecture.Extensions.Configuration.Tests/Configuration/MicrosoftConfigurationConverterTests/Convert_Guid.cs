using System;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_Guid : ConvertBase
	{
		public Convert_Guid(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
		}

		[Fact]
		public void Should_set_property_when_creator_returns_valid_result()
		{
			SetupCreateFromString("31033C8F-FE32-41BF-934B-3C23DC85F28F", new Guid("31033C8F-FE32-41BF-934B-3C23DC85F28F"));

			RoundtripConvert<TestConfiguration<Guid>>("P1", "31033C8F-FE32-41BF-934B-3C23DC85F28F")
				.P1.Should().Be(new Guid("31033C8F-FE32-41BF-934B-3C23DC85F28F"));
		}
	}
}
