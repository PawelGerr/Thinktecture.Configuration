using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Collection : ConvertBase
	{
		public Convert_Collection()
		{
			InstanceCreatorMock.Setup(c => c.CreateArray(It.IsAny<Type>(), It.IsAny<int>()))
				.Returns<Type, int>((type, length) => type.GetTypeInfo().IsGenericType ? (Array) new int[length] : new object[length]);
			InstanceCreatorMock.Setup(c => c.Create(It.IsAny<Type>()))
				.Returns<Type>(type => type.GetTypeInfo().IsGenericType ? (object) new List<int>() : new List<object>());
			InstanceCreatorMock.Setup(c => c.Create(typeof(int), "42")).Returns(42);
			InstanceCreatorMock.Setup(c => c.Create(typeof(int), "43")).Returns(43);
		}

		[Fact]
		public void Should_convert_null_to_empty_collection()
		{
			var result = RoundtripConvert<TestConfiguration<ICollection<int>>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<ICollection<int>>() {P1 = new List<int>()});
		}

		[Fact]
		public void Should_convert_empty_string_to_empty_collection()
		{
			var result = RoundtripConvert<TestConfiguration<ICollection<int>>>("P1", String.Empty);
			result.ShouldBeEquivalentTo(new TestConfiguration<ICollection<int>>() { P1 = new List<int>() });
		}

		[Fact]
		public void Should_convert_collection_to_list()
		{
			var result = RoundtripConvert<TestConfiguration<ICollection<int>>>("P1:0", "42");
			result.P1.Should().BeOfType<List<int>>();
		}

		[Fact]
		public void Should_convert_collection_with_one_value()
		{
			var result = RoundtripConvert<TestConfiguration<ICollection<int>>>("P1:0", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<ICollection<int>>() {P1 = new List<int> {42}});
		}

		[Fact]
		public void Should_convert_collection_with_two_values()
		{
			var result = RoundtripConvert<TestConfiguration<ICollection<int>>>(dictionary =>
			{
				dictionary.Add("P1:0", "42");
				dictionary.Add("P1:1", "43");
			});
			result.ShouldBeEquivalentTo(new TestConfiguration<ICollection<int>>() {P1 = new List<int> {42, 43}});
		}

		[Fact]
		public void Should_convert_collection_with_two_non_adjacent_values()
		{
			var result = RoundtripConvert<TestConfiguration<ICollection<int>>>(dictionary =>
			{
				dictionary.Add("P1:0", "42");
				dictionary.Add("P1:2", "43");
			});
			result.ShouldBeEquivalentTo(new TestConfiguration<ICollection<int>>() {P1 = new List<int> {42, 0, 43}});
		}

		[Fact]
		public void Should_ignore_invalid_indexes()
		{
			var result = RoundtripConvert<TestConfiguration<ICollection<int>>>(dictionary =>
			{
				dictionary.Add("P1:0", "42");
				dictionary.Add("P1:Foo", "1");
				dictionary.Add("P1:2", "43");
			});
			result.ShouldBeEquivalentTo(new TestConfiguration<ICollection<int>>() {P1 = new List<int> {42, 0, 43}});
		}

		[Fact]
		public void Should_ignore_invalid_values()
		{
			var result = RoundtripConvert<TestConfiguration<ICollection<int>>>(dictionary =>
			{
				dictionary.Add("P1:0", "42");
				dictionary.Add("P1:1", "Foo");
				dictionary.Add("P1:2", "43");
			});
			result.ShouldBeEquivalentTo(new TestConfiguration<ICollection<int>>() {P1 = new List<int> {42, 0, 43}});
		}

		[Fact]
		public void Should_convert_enumerable_of_int32()
		{
			var result = RoundtripConvert<TestConfiguration<IEnumerable<int>>>("P1:0", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<IEnumerable<int>>() {P1 = new List<int> {42}});
		}

		[Fact]
		public void Should_convert_list_of_int32()
		{
			var result = RoundtripConvert<TestConfiguration<List<int>>>("P1:0", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<List<int>>() {P1 = new List<int> {42}});
		}

		[Fact]
		public void Should_convert_readonlycollection()
		{
			var result = RoundtripConvert<TestConfiguration<IReadOnlyCollection<int>>>("P1:0", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<IReadOnlyCollection<int>>() {P1 = new List<int> {42}});
		}

		[Fact]
		public void Should_convert_readonlylist()
		{
			var result = RoundtripConvert<TestConfiguration<IReadOnlyList<int>>>("P1:0", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<IReadOnlyList<int>>() {P1 = new List<int> {42}});
		}

		[Fact]
		public void Should_populate_existing_collection()
		{
			var result = RoundtripConvert<TestConfigurationWithInitializedProperty<List<int>>>("P1:0", "42");
			result.ShouldBeEquivalentTo(new TestConfigurationWithInitializedProperty<List<int>>() { P1 = new List<int> { 42 } });
		}
	}
}