using System;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using Thinktecture.TestTypes;
using Xunit;

namespace Thinktecture.Configuration.AutofacCreationJsonConverterTests
{
	public class Create
	{
		private readonly ContainerBuilder _builder;
		private readonly Type _type;

		public Create()
		{
			_type = typeof(ConfigurationWithDefaultCtor);
			_builder = new ContainerBuilder();
		}

		private AutofacCreationJsonConverter CreateConverter()
		{
			return new AutofacCreationJsonConverter(_type, _builder.Build());
		}

		[Fact]
		public void Should_throw_argnull_if_type_is_null()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			CreateConverter()
				.Invoking(c => c.Create(null))
				.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_type_is_unknown()
		{
			CreateConverter()
				.Invoking(c => c.Create(_type))
				.Should().Throw<DependencyResolutionException>();
		}

		[Fact]
		public void Should_create_new_instance_of_known_type()
		{
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsSelf();

			CreateConverter()
				.Create(typeof(ConfigurationWithDefaultCtor))
				.Should()
				.NotBeNull();
		}
	}
}
