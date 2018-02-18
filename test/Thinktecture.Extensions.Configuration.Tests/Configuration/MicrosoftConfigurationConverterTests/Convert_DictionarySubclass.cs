using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_DictionarySubclass : ConvertBase
	{
		public Convert_DictionarySubclass(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(TestDictionary<int, string, string>)))
			                   .Returns(() => new ConversionResult(new TestDictionary<int, string, string>()));
		}

		[Fact]
		public void Should_populate_custom_propery()
		{
			SetupCreateFromString("1", 1);
			SetupCreateFromString("42", 42);
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
			SetupCreateFromString("1", 1);
			SetupCreateFromString("2", 2);
			SetupCreateFromString("42", 42);
			SetupCreateFromString("43", 43);
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
