using System;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationProviderTests
{
	public class Ctor : JsonFileConfigurationProviderTestsBase
	{
		[Fact]
		public void Should_not_throw_argnull_if_token_is_null()
		{
			Action ctor = () => new JsonFileConfigurationProvider(null, ConverterMock.Object);
			ctor.Invoking(c => c())
				.ShouldNotThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_argnull_if_converter_is_null()
		{
			Action ctor = () => new JsonFileConfigurationProvider(null, null);
			ctor.Invoking(c => c())
				.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_not_call_any_members()
		{
			new JsonFileConfigurationProvider(JToken.FromObject(new {}), ConverterMock.Object);
		}
	}
}