using System;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationLoaderTests
{
	public class GetConfiguration : JsonFileConfigurationLoaderTestsBase
	{
		[Fact]
		public void Should_return_null_if_token_is_null()
		{
			CreateProvider(null)
				.GetConfiguration<string>().Should().BeNull();
		}

		[Fact]
		public void Should_throw_if_converter_throws()
		{
			ConverterMock.Setup(c => c.Convert<string>(It.IsAny<JToken>())).Throws<InvalidCastException>();

			CreateProvider(new {})
				.Invoking(p => p.GetConfiguration<string>())
				.ShouldThrow<InvalidCastException>();
		}

		[Fact]
		public void Should_delegate_conversion_to_converter()
		{
			var content = "content";
			ConverterMock.Setup(c => c.Convert<string>(It.IsAny<JToken>())).Returns<JToken>(token => token.Value<string>());

			CreateProvider(content)
				.GetConfiguration<string>()
				.Should().Be(content);

			ConverterMock.Verify(c => c.Convert<string>(It.IsAny<JToken>()), Times.Once);
		}

		[Fact]
		public void Should_select_property_for_deserialization()
		{
			var selectorMock = new Mock<IConfigurationSelector<JToken>>(MockBehavior.Strict);
			selectorMock.Setup(s => s.Select(It.IsAny<JToken>())).Returns<JToken>(token => token["Property"]);
			ConverterMock.Setup(c => c.Convert<string>(It.IsAny<JToken>())).Returns<JToken>(token => token.Value<string>());

			CreateProvider(new {Property = "content"})
				.GetConfiguration<string>(selectorMock.Object)
				.Should().Be("content");
		}

		[Fact]
		public void Should_select_sub_property_for_deserialization()
		{
			var selectorMock = new Mock<IConfigurationSelector<JToken>>(MockBehavior.Strict);
			selectorMock.Setup(s => s.Select(It.IsAny<JToken>())).Returns<JToken>(token => token["Parent"]["Child"]);
			ConverterMock.Setup(c => c.Convert<string>(It.IsAny<JToken>())).Returns<JToken>(token => token.Value<string>());

			CreateProvider(new {Parent = new {Child = "content"}})
				.GetConfiguration<string>(selectorMock.Object)
				.Should().Be("content");
		}
	}
}