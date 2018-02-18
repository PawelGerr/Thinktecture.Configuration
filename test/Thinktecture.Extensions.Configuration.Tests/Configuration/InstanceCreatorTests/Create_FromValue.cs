using System;
using System.Globalization;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.InstanceCreatorTests
{
	public class Create_FromValue
	{
		private TestInstanceCreator _creator;

		public Create_FromValue()
		{
			Initialize(CultureInfo.InvariantCulture);
		}

		private void Initialize(CultureInfo cultureInfo)
		{
			_creator = new TestInstanceCreator(cultureInfo);
		}

		private void Should_create<T>(string input, T output)
		{
			_creator.Create(typeof(T), input)
			        .Value.Should().Be(output);
		}

		private void Should_throw<T>(string input)
		{
			Action action = () => _creator.Create(typeof(T), input);
			action.ShouldThrow<ConfigurationSerializationException>();
		}

		[Fact]
		public void Should_create_string()
		{
			Should_create("foo", "foo");
		}

		[Fact]
		public void Should_create_char()
		{
			Should_create("a", 'a');
		}

		[Fact]
		public void Should_throw_if_value_is_not_a_valid_char()
		{
			Should_throw<char>("foo");
		}

		[Fact]
		public void Should_create_nullable_int()
		{
			Should_create<int?>("42", 42);
		}

		[Fact]
		public void Should_create_nullable_int_from_empty_string()
		{
			Should_create<int?>(String.Empty, null);
		}

		[Fact]
		public void Should_create_nullable_int_from_null()
		{
			Should_create<int?>(null, null);
		}

		[Fact]
		public void Should_create_int()
		{
			Should_create("42", 42);
		}

		[Fact]
		public void Should_create_uint()
		{
			Should_create<uint>("42", 42);
		}

		[Fact]
		public void Should_create_long()
		{
			Should_create("42", 42L);
		}

		[Fact]
		public void Should_create_ulong()
		{
			Should_create<ulong>("42", 42);
		}

		[Fact]
		public void Should_create_sbyte()
		{
			Should_create<sbyte>("42", 42);
		}

		[Fact]
		public void Should_create_byte()
		{
			Should_create<byte>("42", 42);
		}

		[Fact]
		public void Should_create_short()
		{
			Should_create<short>("42", 42);
		}

		[Fact]
		public void Should_create_ushort()
		{
			Should_create<ushort>("42", 42);
		}

		[Fact]
		public void Should_create_decimal()
		{
			Should_create("42.1", 42.1m);
		}

		[Fact]
		public void Should_create_decimal_with_de_de()
		{
			Initialize(new CultureInfo("de-DE"));

			Should_create("42,1", 42.1m);
		}

		[Fact]
		public void Should_create_double()
		{
			Should_create("42.1", 42.1d);
		}

		[Fact]
		public void Should_create_float()
		{
			Should_create("42.1", 42.1f);
		}

		[Fact]
		public void Should_create_double_with_de_de()
		{
			Initialize(new CultureInfo("de-DE"));

			Should_create("42,1", 42.1d);
		}

		[Fact]
		public void Should_create_boolean()
		{
			Should_create("true", true);
		}

		[Fact]
		public void Should_throw_when_empty_string_should_be_converted_to_boolean()
		{
			Should_throw<bool>(String.Empty);
		}

		[Fact]
		public void Should_create_guid()
		{
			Should_create("7EC124D6-B8D7-474E-AF5E-567A3FB36184", new Guid("7EC124D6-B8D7-474E-AF5E-567A3FB36184"));
		}

		[Fact]
		public void Should_create_datetime()
		{
			Should_create("2017-08-01T12:34:56.123", new DateTime(2017, 08, 01, 12, 34, 56, 123, DateTimeKind.Unspecified));
		}

		[Fact]
		public void Should_create_timespan()
		{
			Should_create("1.23:45:56", new TimeSpan(1, 23, 45, 56));
		}

		[Fact]
		public void Should_create_class_with_typeconverter()
		{
			_creator.Create(typeof(ClassWithTypeConverter), "value")
			        .Value.ShouldBeEquivalentTo(new ClassWithTypeConverter() { Prop = "value" });
		}

		[Fact]
		public void Should_return_invalid_result_if_creation_from_non_empty_string_is_not_possible()
		{
			var result = _creator.Create(typeof(TestConfiguration<int>), "value");
			result.IsValid.Should().BeFalse();
		}

		[Fact]
		public void Should_return_invalid_result_if_creation_from_empty_string_is_not_possible()
		{
			var result = _creator.Create(typeof(TestConfiguration<int>), "");
			result.IsValid.Should().BeFalse();
		}

		[Fact]
		public void Should_return_invalid_result_if_creation_from_null_string_is_not_possible()
		{
			var result = _creator.Create(typeof(TestConfiguration<int>), null);
			result.IsValid.Should().BeFalse();
		}

		[Fact]
		public void Should_return_invalid_result_if_creation_of_struct_from_null_string_is_not_possible()
		{
			var result = _creator.Create(typeof(TestStruct), null);
			result.IsValid.Should().BeFalse();
		}

		[Fact]
		public void Should_return_invalid_result_if_creation_of_struct_from_empty_string_is_not_possible()
		{
			var result = _creator.Create(typeof(TestStruct), "");
			result.IsValid.Should().BeFalse();
		}

		[Fact]
		public void Should_return_invalid_result_if_creation_of_nullable_struct_from_empty_string_is_not_possible()
		{
			var result = _creator.Create(typeof(TestStruct?), "");
			result.IsValid.Should().BeFalse();
		}

		[Fact]
		public void Should_return_invalid_result_if_creation_of_nullable_struct_from_null_string_is_not_possible()
		{
			var result = _creator.Create(typeof(TestStruct?), null);
			result.IsValid.Should().BeFalse();
		}
	}
}
