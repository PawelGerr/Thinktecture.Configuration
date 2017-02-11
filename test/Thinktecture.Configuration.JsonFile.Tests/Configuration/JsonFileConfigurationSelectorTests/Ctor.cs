using System;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Configuration.JsonFileConfigurationSelectorTests
{
	public class Ctor
	{
		[Fact]
		public void Should_throw_argnull_if_property_name_is_null()
		{
			Action ctor = () => new JsonFileConfigurationSelector(null);

			ctor.Invoking(c => c())
				.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_argexception_if_property_name_is_empty()
		{
			Action ctor = () => new JsonFileConfigurationSelector(" ");

			ctor.Invoking(c => c())
				.ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Should_throw_if_propertypath_contains_multiple_dots_next_to_each_other()
		{
			Action ctor = () => new JsonFileConfigurationSelector("Parent..Child");

			ctor.Invoking(c => c())
				.ShouldThrow<ArgumentException>();
		}

		[Fact]
		public void Should_initialize_new_instance()
		{
			new JsonFileConfigurationSelector("Property");
		}
	}
}