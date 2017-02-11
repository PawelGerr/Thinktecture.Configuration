using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Thinktecture.TestTypes;
using Xunit;

namespace Thinktecture.Configuration.JsonTokenConverterTests
{
	public class Convert
	{
		private readonly ContainerBuilder _builder;
		private readonly List<Type> _typesToConvertViaAutofac;
		private Func<Type, JsonSerializerSettings> _settingsProvider;

		public Convert()
		{
			_builder = new ContainerBuilder();
			_typesToConvertViaAutofac = new List<Type>();
		}

		private AutofacJsonTokenConverter Create()
		{
			return new AutofacJsonTokenConverter(_builder.Build(), _typesToConvertViaAutofac, _settingsProvider);
		}

		[Fact]
		public void Should_throw_argnull_if_jtoken_is_null()
		{
			Create()
				.Invoking(c => c.Convert<ConfigurationWithDefaultCtor>(null))
				.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_convert_concrete_type_without_autofac_using_default_ctor()
		{
			var token = JToken.FromObject(new {});

			var config = Create().Convert<ConfigurationWithDefaultCtor>(token);
			config.Should().NotBeNull();
		}

		[Fact]
		public void Should_throw_when_converting_interface_without_autofac()
		{
			var token = JToken.FromObject(new {});

			Create()
				.Invoking(c => c.Convert<IConfigurationWithDefaultCtor>(token))
				.ShouldThrow<JsonSerializationException>();
		}

		[Fact]
		public void Should_throw_when_converting_concrete_type_with_autofac_without_registration()
		{
			_typesToConvertViaAutofac.Add(typeof(ConfigurationWithDefaultCtor));
			var token = JToken.FromObject(new {});

			Create()
				.Invoking(c => c.Convert<ConfigurationWithDefaultCtor>(token))
				.ShouldThrow<ComponentNotRegisteredException>();
		}

		[Fact]
		public void Should_throw_when_converting_interface_with_autofac_without_registration()
		{
			_typesToConvertViaAutofac.Add(typeof(IConfigurationWithDefaultCtor));
			var token = JToken.FromObject(new {});

			Create()
				.Invoking(c => c.Convert<IConfigurationWithDefaultCtor>(token))
				.ShouldThrow<ComponentNotRegisteredException>();
		}

		[Fact]
		public void Should_convert_concrete_type_with_autofac_using_default_ctor()
		{
			_typesToConvertViaAutofac.Add(typeof(ConfigurationWithDefaultCtor));
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsSelf();
			var token = JToken.FromObject(new {});

			var config = Create().Convert<ConfigurationWithDefaultCtor>(token);
			config.Should().NotBeNull();
		}

		[Fact]
		public void Should_convert_interface_with_autofac_using_default_ctor()
		{
			_typesToConvertViaAutofac.Add(typeof(IConfigurationWithDefaultCtor));
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsImplementedInterfaces();
			var token = JToken.FromObject(new {});

			var config = Create().Convert<IConfigurationWithDefaultCtor>(token);
			config.Should().NotBeNull();
		}

		[Fact]
		public void Should_convert_concrete_type_without_autofac_having_non_default_ctor()
		{
			var token = JToken.FromObject(new {});
			var config = Create().Convert<ConfigurationWithNonDefaultCtor>(token);
			config.Should().NotBeNull();
			config.SimpleDependency.Should().BeNull();
		}

		[Fact]
		public void Should_throw_when_converting_concrete_type_with_autofac_without_registration_of_dependency()
		{
			_typesToConvertViaAutofac.Add(typeof(ConfigurationWithNonDefaultCtor));
			_builder.RegisterType<ConfigurationWithNonDefaultCtor>().AsSelf();
			var token = JToken.FromObject(new {});

			Create()
				.Invoking(c => c.Convert<ConfigurationWithNonDefaultCtor>(token))
				.ShouldThrow<DependencyResolutionException>();
		}

		[Fact]
		public void Should_convert_concrete_type_with_autofac_having_non_default_ctor()
		{
			var simpleDep = new SimpleDependency();
			_typesToConvertViaAutofac.Add(typeof(ConfigurationWithNonDefaultCtor));
			_builder.RegisterType<ConfigurationWithNonDefaultCtor>().AsSelf();
			_builder.RegisterInstance(simpleDep);
			var token = JToken.FromObject(new {});

			var config = Create().Convert<ConfigurationWithNonDefaultCtor>(token);
			config.Should().NotBeNull();
			config.SimpleDependency.Should().Be(simpleDep);
		}

		[Fact]
		public void Should_convert_interface_with_autofac_having_non_default_ctor()
		{
			var simpleDep = new SimpleDependency();
			_typesToConvertViaAutofac.Add(typeof(IConfigurationWithNonDefaultCtor));
			_builder.RegisterType<ConfigurationWithNonDefaultCtor>().AsImplementedInterfaces();
			_builder.RegisterInstance(simpleDep);
			var token = JToken.FromObject(new {});

			var config = Create().Convert<IConfigurationWithNonDefaultCtor>(token);
			config.Should().NotBeNull();
			config.SimpleDependency.Should().Be(simpleDep);
		}

		[Fact]
		public void Should_convert_concrete_type_without_autofac_having_concrete_propery()
		{
			var token = JToken.FromObject(new {InnerConfiguration = new {}});

			var config = Create().Convert<ConfigurationWithPropertyOfConcreteType>(token);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}

		[Fact]
		public void Should_should_throw_when_converting_concrete_type_without_autofac_having_abstract_propery()
		{
			var token = JToken.FromObject(new {InnerConfiguration = new {}});

			Create()
				.Invoking(c => c.Convert<ConfigurationWithPropertyOfAbstractType>(token))
				.ShouldThrow<JsonSerializationException>();
		}

		[Fact]
		public void Should_convert_concrete_type_with_autofac_having_concrete_propery()
		{
			_typesToConvertViaAutofac.Add(typeof(ConfigurationWithPropertyOfConcreteType));
			_typesToConvertViaAutofac.Add(typeof(ConfigurationWithDefaultCtor));
			_builder.RegisterType<ConfigurationWithPropertyOfConcreteType>().AsSelf();
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsSelf();
			var token = JToken.FromObject(new {InnerConfiguration = new {}});

			var config = Create().Convert<ConfigurationWithPropertyOfConcreteType>(token);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}

		[Fact]
		public void Should_convert_concrete_type_with_autofac_having_abstract_propery()
		{
			_typesToConvertViaAutofac.Add(typeof(ConfigurationWithPropertyOfAbstractType));
			_typesToConvertViaAutofac.Add(typeof(IConfigurationWithDefaultCtor));
			_builder.RegisterType<ConfigurationWithPropertyOfAbstractType>().AsSelf();
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsImplementedInterfaces();
			var token = JToken.FromObject(new {InnerConfiguration = new {}});

			var config = Create().Convert<ConfigurationWithPropertyOfAbstractType>(token);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}

		[Fact]
		public void Should_convert_abstract_type_with_autofac_having_abstract_propery()
		{
			_typesToConvertViaAutofac.Add(typeof(IConfigurationWithPropertyOfAbstractType));
			_typesToConvertViaAutofac.Add(typeof(IConfigurationWithDefaultCtor));
			_builder.RegisterType<ConfigurationWithPropertyOfAbstractType>().AsImplementedInterfaces();
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsImplementedInterfaces();
			var token = JToken.FromObject(new {InnerConfiguration = new {}});

			var config = Create().Convert<IConfigurationWithPropertyOfAbstractType>(token);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}
	}
}