using System;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_String : ConvertBase
	{
		public Convert_String()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(string), It.IsAny<string>()))
				.Returns<Type, string>((type, value) => value);
		}

		[Fact]
		public void Should_convert_non_empty_string_property()
		{
			var result = RoundtripConvert<TestConfiguration<string>>("P1", "value");
			result.ShouldBeEquivalentTo(new TestConfiguration<string>() {P1 = "value"});
		}

		[Fact]
		public void Should_convert_empty_string_property()
		{
			var result = RoundtripConvert<TestConfiguration<string>>("P1", String.Empty);
			result.ShouldBeEquivalentTo(new TestConfiguration<string>() {P1 = String.Empty});
		}

		[Fact]
		public void Should_convert_null_string_property()
		{
			var result = RoundtripConvert<TestConfiguration<string>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<string>() {P1 = null});
		}
	}
}