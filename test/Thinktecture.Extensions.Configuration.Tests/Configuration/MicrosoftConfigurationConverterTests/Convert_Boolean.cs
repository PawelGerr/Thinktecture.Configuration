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
	public class Convert_Boolean : ConvertBase
	{
		[Fact]
		public void Should_convert_true_string()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(bool), "True")).Returns(true);

			var result = RoundtripConvert<TestConfiguration<bool>>("P1", "True");
			result.ShouldBeEquivalentTo(new TestConfiguration<bool>() {P1 = true});
		}

		[Fact]
		public void Should_convert_empty_string_property()
		{
			Action action = () => RoundtripConvert<TestConfiguration<bool>>("P1", String.Empty);
			action.ShouldThrow<InvalidOperationException>();
		}

		[Fact]
		public void Should_convert_null_string_property()
		{
			var result = RoundtripConvert<TestConfiguration<bool>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<bool>() {P1 = false});
		}
	}
}