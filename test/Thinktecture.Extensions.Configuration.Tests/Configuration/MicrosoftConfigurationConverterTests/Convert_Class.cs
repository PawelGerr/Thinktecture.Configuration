using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Class : ConvertBase
	{
		[Fact]
		public void Should_return_empty_object_if_keys_of_all_properties_are_missing()
		{
			var result = RoundtripConvert<TestConfiguration<decimal>>(dictionary => { });
			result.ShouldBeEquivalentTo(new TestConfiguration<decimal>());
		}

		[Fact]
		public void Should_convert_inner_complex_property()
		{
			InstanceCreatorMock.Setup(creator => creator.Create(It.Is<Type>(type => type == typeof(TestConfiguration<decimal>))))
				.Returns(() => new TestConfiguration<decimal>());
			InstanceCreatorMock.Setup(c => c.Create(typeof(decimal), "42")).Returns(42m);

			var result = RoundtripConvert(new TestConfiguration<TestConfiguration<decimal>>()
			{
				P1 = new TestConfiguration<decimal>() { P1 = 42 }
			});

			result.ShouldBeEquivalentTo(new TestConfiguration<TestConfiguration<decimal>>()
			{
				P1 = new TestConfiguration<decimal>() { P1 = 42 }
			});
		}
	}
}