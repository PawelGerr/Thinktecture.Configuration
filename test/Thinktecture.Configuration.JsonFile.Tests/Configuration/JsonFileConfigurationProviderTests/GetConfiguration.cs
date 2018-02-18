using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationProviderTests
{
	public class GetConfiguration : JsonFileConfigurationProviderTestsBase
	{
		[Fact]
		public void Should_throw_if_converter_throws()
		{
			ConverterMock.Setup(c => c.Convert<string>(It.IsAny<JToken[]>())).Throws<InvalidCastException>();

			CreateProvider(new { })
				.Invoking(p => p.GetConfiguration<string>())
				.ShouldThrow<InvalidCastException>();
		}

		[Fact]
		public void Should_delegate_conversion_to_converter()
		{
			ConverterMock.Setup(c => c.Convert<string>(It.IsAny<JToken[]>())).Returns<JToken[]>(tokens => String.Concat(tokens.Select(s => s.Value<string>())));

			CreateProvider("con", "tent")
				.GetConfiguration<string>()
				.Should().Be("content");

			ConverterMock.Verify(c => c.Convert<string>(It.IsAny<JToken[]>()), Times.Once);
		}

		[Fact]
		public void Should_select_property_for_deserialization()
		{
			var selectorMock = new Mock<IConfigurationSelector<JToken, JToken>>(MockBehavior.Strict);
			selectorMock.Setup(s => s.Select(It.IsAny<JToken>())).Returns<JToken>(token => token["Property"]);
			ConverterMock.Setup(c => c.Convert<string>(It.IsAny<JToken[]>())).Returns<JToken[]>(tokens => tokens.First().Value<string>());

			CreateProvider(new { Property = "content" })
				.GetConfiguration<string>(selectorMock.Object)
				.Should().Be("content");
		}

		[Fact]
		public void Should_select_sub_property_for_deserialization()
		{
			var selectorMock = new Mock<IConfigurationSelector<JToken, JToken>>(MockBehavior.Strict);
			selectorMock.Setup(s => s.Select(It.IsAny<JToken>())).Returns<JToken>(token => token["Parent"]["Child"]);
			ConverterMock.Setup(c => c.Convert<string>(It.IsAny<JToken[]>())).Returns<JToken[]>(tokens => tokens.First().Value<string>());

			CreateProvider(new { Parent = new { Child = "content" } })
				.GetConfiguration<string>(selectorMock.Object)
				.Should().Be("content");
		}
	}
}
