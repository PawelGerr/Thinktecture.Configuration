using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	// ReSharper disable once InconsistentNaming
	public class Convert_CollectionSubclass : ConvertBase
	{
		public Convert_CollectionSubclass(ITestOutputHelper outputHelper)
			: base(outputHelper)
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(TestList<int, string>))).Returns(() => new ConversionResult(new TestList<int, string>()));
		}

		[Fact]
		public void Should_populate_custom_propery_on_collection()
		{
			SetupCreateFromString("1", 1);

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
			SetupCreateFromString("1", 1);
			SetupCreateFromString("2", 2);

			RoundtripConvert<TestConfiguration<TestList<int, string>>>(dictionary =>
				{
					dictionary.Add("P1:0", "1");
					dictionary.Add("P1:1", "2");
					dictionary.Add("P1:Property", "value");
				})
				.P1.Should().BeEquivalentTo(new TestList<int, string>() { 1, 2 });
		}
	}
}
