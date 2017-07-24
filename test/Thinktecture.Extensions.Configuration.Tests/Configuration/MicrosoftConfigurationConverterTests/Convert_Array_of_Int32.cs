using System;
using FluentAssertions;
using Moq;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public class Convert_Array_of_Int32 : ConvertBase
	{
		public Convert_Array_of_Int32()
		{
			InstanceCreatorMock.Setup(c => c.CreateArray(typeof(int), It.IsAny<int>()))
				.Returns<Type, int>((type, length) => new int[length]);

			InstanceCreatorMock.Setup(c => c.Create(typeof(int), "42")).Returns(42);
			InstanceCreatorMock.Setup(c => c.Create(typeof(int), "43")).Returns(43);
		}

		[Fact]
		public void Should_convert_null_to_empty_array()
		{
			var result = RoundtripConvert<TestConfiguration<int[]>>("P1", null);
			result.ShouldBeEquivalentTo(new TestConfiguration<int[]>() { P1 = new int[0] });
		}

		[Fact]
		public void Should_convert_empty_string_to_empty_array()
		{
			var result = RoundtripConvert<TestConfiguration<int[]>>("P1", String.Empty);
			result.ShouldBeEquivalentTo(new TestConfiguration<int[]>() {P1 = new int[0]});
		}

		[Fact]
		public void Should_convert_array_with_one_value()
		{
			var result = RoundtripConvert<TestConfiguration<int[]>>("P1:0", "42");
			result.ShouldBeEquivalentTo(new TestConfiguration<int[]>() {P1 = new[] {42}});
		}

		[Fact]
		public void Should_convert_array_with_two_values()
		{
			var result = RoundtripConvert<TestConfiguration<int[]>>(dictionary =>
			{
				dictionary.Add("P1:0", "42");
				dictionary.Add("P1:1", "43");
			});
			result.ShouldBeEquivalentTo(new TestConfiguration<int[]>() {P1 = new[] {42, 43}});
		}

		[Fact]
		public void Should_convert_array_with_two_non_adjacent_values()
		{
			var result = RoundtripConvert<TestConfiguration<int[]>>(dictionary =>
			{
				dictionary.Add("P1:0", "42");
				dictionary.Add("P1:2", "43");
			});
			result.ShouldBeEquivalentTo(new TestConfiguration<int[]>() {P1 = new[] {42, 0, 43}});
		}

		[Fact]
		public void Should_ignore_invalid_indexes()
		{
			var result = RoundtripConvert<TestConfiguration<int[]>>(dictionary =>
			{
				dictionary.Add("P1:0", "42");
				dictionary.Add("P1:Foo", "1");
				dictionary.Add("P1:2", "43");
			});
			result.ShouldBeEquivalentTo(new TestConfiguration<int[]>() {P1 = new[] {42, 0, 43}});
		}

		[Fact]
		public void Should_ignore_invalid_values()
		{
			var result = RoundtripConvert<TestConfiguration<int[]>>(dictionary =>
			{
				dictionary.Add("P1:0", "42");
				dictionary.Add("P1:1", "Foo");
				dictionary.Add("P1:2", "43");
			});
			result.ShouldBeEquivalentTo(new TestConfiguration<int[]>() {P1 = new[] {42, 0, 43}});
		}
	}
}