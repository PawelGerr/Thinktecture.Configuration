using System;
using System.Collections.Generic;
using System.Reflection;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Dictionary : ConvertBase
	{
		public Convert_Dictionary()
		{
			InstanceCreatorMock.Setup(c => c.Create(It.IsAny<Type>()))
				.Returns<Type>(type => new Dictionary<string, int>());
			InstanceCreatorMock.Setup(c => c.Create(typeof(int), "42")).Returns(42);
		}

		[Fact]
		public void Should_convert_empty_config_to_null()
		{
			var result = RoundtripConvert<TestConfiguration<Dictionary<string, int>>>(dictionary => {});
			result.ShouldBeEquivalentTo(new TestConfiguration<Dictionary<string, int>>() {P1 = null});
		}

		[Fact]
		public void Should_convert_null_string_to_empty_dictionary()
		{
			var result = RoundtripConvert<TestConfiguration<Dictionary<string, int>>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<Dictionary<string, int>>() {P1 = new Dictionary<string, int>()});
		}

		[Fact]
		public void Should_convert_empty_string_to_empty_dictionary()
		{
			var result = RoundtripConvert<TestConfiguration<Dictionary<string, int>>>("P1", "");
			result.ShouldBeEquivalentTo(new TestConfiguration<Dictionary<string, int>>() {P1 = new Dictionary<string, int>()});
		}

		[Fact]
		public void Should_convert_dictionary_with_one_value()
		{
			var result = RoundtripConvert<TestConfiguration<Dictionary<string, int>>>("P1:foo", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<Dictionary<string, int>>() {P1 = new Dictionary<string, int>() {{"foo", 42}}});
		}

		[Fact]
		public void Should_convert_idictionary_with_one_value()
		{
			var result = RoundtripConvert<TestConfiguration<IDictionary<string, int>>>("P1:foo", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<IDictionary<string, int>>() {P1 = new Dictionary<string, int>() {{"foo", 42}}});
		}

		[Fact]
		public void Should_convert_ireadonlydictionary_with_one_value()
		{
			var result = RoundtripConvert<TestConfiguration<IReadOnlyDictionary<string, int>>>("P1:foo", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<IReadOnlyDictionary<string, int>>() {P1 = new Dictionary<string, int>() {{"foo", 42}}});
		}
	}
}