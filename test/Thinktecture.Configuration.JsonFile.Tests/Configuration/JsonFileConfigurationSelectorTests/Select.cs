using System;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationSelectorTests
{
	public class Select
	{
		private static JsonFileConfigurationSelector CreateSelector(string propertyPath)
		{
			return new JsonFileConfigurationSelector(propertyPath);
		}

		[Fact]
		public void Should_throw_if_token_is_null()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			CreateSelector("Property")
				.Invoking(s => s.Select(null))
				.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Should_return_null_if_property_not_exists()
		{
			CreateSelector("Property")
				.Select(JToken.FromObject(new { }))
				.Should().BeNull();
		}

		[Fact]
		public void Should_return_null_token_if_value_of_property_is_null()
		{
			// ReSharper disable once PossibleNullReferenceException
			CreateSelector("Property")
				.Select(JToken.FromObject(new { Property = (string)null }))
				.Type.Should().Be(JTokenType.Null);
		}

		[Fact]
		public void Should_return_property()
		{
			CreateSelector("Property")
				.Select(JToken.FromObject(new { Property = "content" }))
				.Value<string>().Should().Be("content");
		}

		[Fact]
		public void Should_return_property_case_insensitive()
		{
			CreateSelector("property")
				.Select(JToken.FromObject(new { Property = "content" }))
				.Value<string>().Should().Be("content");
		}

		[Fact]
		public void Should_return_sub_property()
		{
			CreateSelector("Parent.Child")
				.Select(JToken.FromObject(new { Parent = new { Child = "content" } }))
				.Value<string>().Should().Be("content");
		}
	}
}
