using System;
using System.Collections.Generic;
using System.Linq;
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
		private readonly List<AutofacJsonTokenConverterType> _typesToConvertViaAutofac;

		public Convert()
		{
			_builder = new ContainerBuilder();
			_typesToConvertViaAutofac = new List<AutofacJsonTokenConverterType>();
		}

		private AutofacJsonTokenConverter Create()
		{
			return new AutofacJsonTokenConverter(_builder.Build(), _typesToConvertViaAutofac);
		}

		private JToken[] GetTokens(params object[] objects)
		{
			return objects.Select(o => o == null ? null : JToken.FromObject(o)).ToArray();
		}

		[Fact]
		public void Should_throw_argnull_if_jtokens_are_null()
		{
			Create()
				.Invoking(c => c.Convert<ConfigurationWithDefaultCtor>(null))
				.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_return_null_if_token_is_null()
		{
			var config = Create().Convert<ConfigurationWithDefaultCtor>(new JToken[] {null});
			config.Should().BeNull();
		}

		[Fact]
		public void Should_convert_concrete_type_without_autofac_using_default_ctor()
		{
			var tokens = GetTokens(new {});

			var config = Create().Convert<ConfigurationWithDefaultCtor>(tokens);
			config.Should().NotBeNull();
		}

		[Fact]
		public void Should_return_null_if_the_first_token_is_null()
		{
			var tokens = GetTokens(null, new {});

			var config = Create().Convert<ConfigurationWithDefaultCtor>(tokens);
			config.Should().BeNull();
		}

		[Fact]
		public void Should_throw_when_converting_interface_without_autofac()
		{
			var tokens = GetTokens(new {});

			Create()
				.Invoking(c => c.Convert<IConfigurationWithDefaultCtor>(tokens))
				.ShouldThrow<JsonSerializationException>();
		}

		[Fact]
		public void Should_throw_when_converting_concrete_type_with_autofac_without_registration()
		{
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(ConfigurationWithDefaultCtor)));
			var tokens = GetTokens(new {});

			Create()
				.Invoking(c => c.Convert<ConfigurationWithDefaultCtor>(tokens))
				.ShouldThrow<ComponentNotRegisteredException>();
		}

		[Fact]
		public void Should_throw_when_converting_interface_with_autofac_without_registration()
		{
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(IConfigurationWithDefaultCtor)));
			var tokens = GetTokens(new {});

			Create()
				.Invoking(c => c.Convert<IConfigurationWithDefaultCtor>(tokens))
				.ShouldThrow<ComponentNotRegisteredException>();
		}

		[Fact]
		public void Should_convert_concrete_type_with_autofac_using_default_ctor()
		{
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(ConfigurationWithDefaultCtor)));
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsSelf();
			var tokens = GetTokens(new {});

			var config = Create().Convert<ConfigurationWithDefaultCtor>(tokens);
			config.Should().NotBeNull();
		}

		[Fact]
		public void Should_convert_interface_with_autofac_using_default_ctor()
		{
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(IConfigurationWithDefaultCtor)));
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsImplementedInterfaces();
			var tokens = GetTokens(new {});

			var config = Create().Convert<IConfigurationWithDefaultCtor>(tokens);
			config.Should().NotBeNull();
		}

		[Fact]
		public void Should_convert_concrete_type_without_autofac_having_non_default_ctor()
		{
			var tokens = GetTokens(new {});
			var config = Create().Convert<ConfigurationWithNonDefaultCtor>(tokens);
			config.Should().NotBeNull();
			config.SimpleDependency.Should().BeNull();
		}

		[Fact]
		public void Should_throw_when_converting_concrete_type_with_autofac_without_registration_of_dependency()
		{
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(ConfigurationWithNonDefaultCtor)));
			_builder.RegisterType<ConfigurationWithNonDefaultCtor>().AsSelf();
			var tokens = GetTokens(new {});

			Create()
				.Invoking(c => c.Convert<ConfigurationWithNonDefaultCtor>(tokens))
				.ShouldThrow<DependencyResolutionException>();
		}

		[Fact]
		public void Should_convert_concrete_type_with_autofac_having_non_default_ctor()
		{
			var simpleDep = new SimpleDependency();
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(ConfigurationWithNonDefaultCtor)));
			_builder.RegisterType<ConfigurationWithNonDefaultCtor>().AsSelf();
			_builder.RegisterInstance(simpleDep);
			var tokens = GetTokens(new {});

			var config = Create().Convert<ConfigurationWithNonDefaultCtor>(tokens);
			config.Should().NotBeNull();
			config.SimpleDependency.Should().Be(simpleDep);
		}

		[Fact]
		public void Should_convert_interface_with_autofac_having_non_default_ctor()
		{
			var simpleDep = new SimpleDependency();
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(IConfigurationWithNonDefaultCtor)));
			_builder.RegisterType<ConfigurationWithNonDefaultCtor>().AsImplementedInterfaces();
			_builder.RegisterInstance(simpleDep);
			var tokens = GetTokens(new {});

			var config = Create().Convert<IConfigurationWithNonDefaultCtor>(tokens);
			config.Should().NotBeNull();
			config.SimpleDependency.Should().Be(simpleDep);
		}

		[Fact]
		public void Should_convert_concrete_type_without_autofac_having_concrete_propery()
		{
			var tokens = GetTokens(new {InnerConfiguration = new {}});

			var config = Create().Convert<ConfigurationWithPropertyOfConcreteType>(tokens);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}

		[Fact]
		public void Should_set_inner_property_to_null_by_override()
		{
			var tokens = GetTokens(new {InnerConfiguration = new ConfigurationWithDefaultCtor()}, new {InnerConfiguration = (ConfigurationWithDefaultCtor) null});

			var config = Create().Convert<ConfigurationWithPropertyOfConcreteType>(tokens);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().BeNull();
		}

		[Fact]
		public void Should_set_inner_property_by_override()
		{
			var tokens = GetTokens(new {InnerConfiguration = (ConfigurationWithDefaultCtor) null}, new {InnerConfiguration = new ConfigurationWithDefaultCtor()});

			var config = Create().Convert<ConfigurationWithPropertyOfConcreteType>(tokens);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}

		[Fact]
		public void Should_dont_change_inner_property_by_override()
		{
			var tokens = GetTokens(new {InnerConfiguration = new ConfigurationWithDefaultCtor()}, new {});

			var config = Create().Convert<ConfigurationWithPropertyOfConcreteType>(tokens);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}

		[Fact]
		public void Should_should_throw_when_converting_concrete_type_without_autofac_having_abstract_propery()
		{
			var tokens = GetTokens(new {InnerConfiguration = new {}});

			Create()
				.Invoking(c => c.Convert<ConfigurationWithPropertyOfAbstractType>(tokens))
				.ShouldThrow<JsonSerializationException>();
		}

		[Fact]
		public void Should_convert_concrete_type_with_autofac_having_concrete_propery()
		{
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(ConfigurationWithPropertyOfConcreteType)));
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(ConfigurationWithDefaultCtor)));
			_builder.RegisterType<ConfigurationWithPropertyOfConcreteType>().AsSelf();
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsSelf();
			var tokens = GetTokens(new {InnerConfiguration = new {}});

			var config = Create().Convert<ConfigurationWithPropertyOfConcreteType>(tokens);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}

		[Fact]
		public void Should_convert_concrete_type_with_autofac_having_abstract_propery()
		{
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(ConfigurationWithPropertyOfAbstractType)));
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(IConfigurationWithDefaultCtor)));
			_builder.RegisterType<ConfigurationWithPropertyOfAbstractType>().AsSelf();
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsImplementedInterfaces();
			var tokens = GetTokens(new {InnerConfiguration = new {}});

			var config = Create().Convert<ConfigurationWithPropertyOfAbstractType>(tokens);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}

		[Fact]
		public void Should_convert_abstract_type_with_autofac_having_abstract_propery()
		{
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(IConfigurationWithPropertyOfAbstractType)));
			_typesToConvertViaAutofac.Add(new AutofacJsonTokenConverterType(typeof(IConfigurationWithDefaultCtor)));
			_builder.RegisterType<ConfigurationWithPropertyOfAbstractType>().AsImplementedInterfaces();
			_builder.RegisterType<ConfigurationWithDefaultCtor>().AsImplementedInterfaces();
			var tokens = GetTokens(new {InnerConfiguration = new {}});

			var config = Create().Convert<IConfigurationWithPropertyOfAbstractType>(tokens);
			config.Should().NotBeNull();
			config.InnerConfiguration.Should().NotBeNull();
		}
	}
}