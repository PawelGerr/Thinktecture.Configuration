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
			InstanceCreatorMock.Setup(c => c.Create(It.IsAny<Type>()))
				.Returns<Type>(type => new ConversionResult(type.GetTypeInfo().IsGenericType ? (object) new List<int>() : new List<object>()));
		}

		[Fact]
		public void Should_convert_null_to_null()
		{
			RoundtripConvert<TestConfiguration<ICollection<int>>>("P1", null)
				.P1.Should().BeNull();
		}

		[Fact]
		public void Should_convert_empty_string_to_null()
		{
			RoundtripConvert<TestConfiguration<ICollection<int>>>(c=> c.P1 = null)
				.P1.Should().BeNull();
		}

		[Fact]
		public void Should_convert_collection_to_list()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<ICollection<int>>>("P1:0", "42")
				.P1.Should().BeOfType<List<int>>();
		}

		[Fact]
		public void Should_convert_collection_with_one_value()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<ICollection<int>>>("P1:0", "42")
				.P1.ShouldBeEquivalentTo(new List<int> {42});
		}

		[Fact]
		public void Should_convert_collection_with_two_values()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));
			SetupCreateFromString<int>("43", new ConversionResult(43));

			RoundtripConvert<TestConfiguration<ICollection<int>>>(dictionary =>
				{
					dictionary.Add("P1:0", "42");
					dictionary.Add("P1:1", "43");
				})
				.P1.ShouldBeEquivalentTo(new List<int> {42, 43});
		}

		[Fact]
		public void Should_convert_collection_with_two_non_adjacent_values()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));
			SetupCreateFromString<int>("43", new ConversionResult(43));

			RoundtripConvert<TestConfiguration<ICollection<int>>>(dictionary =>
				{
					dictionary.Add("P1:0", "42");
					dictionary.Add("P1:2", "43");
				})
				.P1.ShouldBeEquivalentTo(new List<int> {42, 0, 43});
		}

		[Fact]
		public void Should_ignore_invalid_indexes()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));
			SetupCreateFromString<int>("43", new ConversionResult(43));

			RoundtripConvert<TestConfiguration<ICollection<int>>>(dictionary =>
				{
					dictionary.Add("P1:0", "42");
					dictionary.Add("P1:Foo", "1");
					dictionary.Add("P1:2", "43");
				})
				.P1.ShouldBeEquivalentTo(new List<int> {42, 0, 43});
		}

		[Fact]
		public void Should_ignore_invalid_values()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));
			SetupCreateFromString<int>("43", new ConversionResult(43));
			SetupCreateFromString<int>("Foo", ConversionResult.Invalid);

			RoundtripConvert<TestConfiguration<ICollection<int>>>(dictionary =>
				{
					dictionary.Add("P1:0", "42");
					dictionary.Add("P1:1", "Foo");
					dictionary.Add("P1:2", "43");
				})
				.P1.ShouldBeEquivalentTo(new List<int> {42, 0, 43});
		}

		[Fact]
		public void Should_convert_enumerable_of_int32()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<IEnumerable<int>>>("P1:0", "42")
				.P1.ShouldBeEquivalentTo(new List<int> {42});
		}

		[Fact]
		public void Should_convert_list_of_int32()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<List<int>>>("P1:0", "42")
				.P1.ShouldBeEquivalentTo(new List<int> {42});
		}

		[Fact]
		public void Should_convert_readonlycollection()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<IReadOnlyCollection<int>>>("P1:0", "42")
				.P1.ShouldBeEquivalentTo(new List<int> {42});
		}

		[Fact]
		public void Should_convert_readonlylist()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));

			RoundtripConvert<TestConfiguration<IReadOnlyList<int>>>("P1:0", "42")
				.P1.ShouldBeEquivalentTo(new List<int> {42});
		}

		[Fact]
		public void Should_populate_existing_collection()
		{
			SetupCreateFromString<int>("42", new ConversionResult(42));

			RoundtripConvert<TestConfigurationWithInitializedProperty<List<int>>>("P1:0", "42")
				.P1.ShouldBeEquivalentTo(new List<int> {42});
		}
	}
}