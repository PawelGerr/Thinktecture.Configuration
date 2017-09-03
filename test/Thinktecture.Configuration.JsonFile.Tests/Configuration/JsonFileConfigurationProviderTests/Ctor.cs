using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationProviderTests
{
	[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
	public class Ctor : JsonFileConfigurationProviderTestsBase
	{
		[Fact]
		public void Should_throw_argnull_if_token_collection_is_null()
		{
			Action ctor = () => new JsonFileConfigurationProvider(null, ConverterMock.Object);
			ctor.Invoking(c => c())
				.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_token_collection_is_empty()
		{
			Action ctor = () => new JsonFileConfigurationProvider(new JToken[0], ConverterMock.Object);
			ctor.Invoking(c => c())
				.ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Should_not_throw_if_token_is_null()
		{
			Action ctor = () => new JsonFileConfigurationProvider(new JToken[] { null }, ConverterMock.Object);
			ctor.Invoking(c => c())
				.ShouldNotThrow<Exception>();
		}

		[Fact]
		public void Should_throw_argnull_if_converter_is_null()
		{
			Action ctor = () => new JsonFileConfigurationProvider(GetTokens(new { }), null);
			ctor.Invoking(c => c())
				.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_not_call_any_members()
		{
			new JsonFileConfigurationProvider(GetTokens(new { }), ConverterMock.Object);
		}

		[Fact]
		public void Should_not_call_any_members_getting_2_tokens()
		{
			new JsonFileConfigurationProvider(GetTokens(new { }, new { }), ConverterMock.Object);
		}
	}
}
