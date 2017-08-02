using System;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_CollectionSubclass : ConvertBase
	{
		public Convert_CollectionSubclass()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(TestList<int, string>))).Returns(() => new ConversionResult(new TestList<int, string>()));
		}

		[Fact]
		public void Should_populate_custom_propery_on_collection()
		{
			SetupCreateFromString<int>("1", new ConversionResult(1));

			RoundtripConvert<TestConfiguration<TestList<int, string>>>(dictionary =>
				{
					dictionary.Add("P1:0", "1");
					dictionary.Add("P1:Property", "value");
				})
				.P1.Property.Should().Be("value");
		}

		[Fact]
		public void Should_populate_collection_values()
		{
			SetupCreateFromString<int>("1", new ConversionResult(1));
			SetupCreateFromString<int>("2", new ConversionResult(2));

			RoundtripConvert<TestConfiguration<TestList<int, string>>>(dictionary =>
				{
					dictionary.Add("P1:0", "1");
					dictionary.Add("P1:1", "2");
					dictionary.Add("P1:Property", "value");
				})
				.P1.ShouldAllBeEquivalentTo(new TestList<int, string>() {1, 2});
		}
	}

	public class Convert_DictionarySubclass : ConvertBase
	{
		public Convert_DictionarySubclass()
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(TestDictionary<int, string, string>)))
				.Returns(() => new ConversionResult(new TestDictionary<int, string, string>()));
		}

		[Fact]
		public void Should_populate_custom_propery()
		{
			SetupCreateFromString<int>("1", new ConversionResult(1));
			SetupCreateFromString<int>("42", new ConversionResult(42));
			SetupCreateFromString<int>("Property", ConversionResult.Invalid);

			RoundtripConvert<TestConfiguration<TestDictionary<int, string, string>>>(dictionary =>
				{
					dictionary.Add("P1:1", "42");
					dictionary.Add("P1:Property", "value");
				})
				.P1.Property.Should().Be("value");
		}

		[Fact]
		public void Should_populate_collection_values()
		{
			SetupCreateFromString<int>("1", new ConversionResult(1));
			SetupCreateFromString<int>("2", new ConversionResult(2));
			SetupCreateFromString<int>("42", new ConversionResult(42));
			SetupCreateFromString<int>("43", new ConversionResult(43));
			SetupCreateFromString<int>("Property", ConversionResult.Invalid);

			RoundtripConvert<TestConfiguration<TestDictionary<int, string, string>>>(dictionary =>
				{
					dictionary.Add("P1:1", "42");
					dictionary.Add("P1:2", "43");
					dictionary.Add("P1:Property", "value");
				})
				.P1.Should().HaveCount(2)
				.And.Contain(1, "42")
				.And.Contain(2, "43");
		}
	}
}