using System;
using Autofac;
using Autofac.Core;
using FluentAssertions;
using Moq;
using Newtonsoft.Json.Linq;
using Thinktecture.Configuration;
using Thinktecture.TestTypes;
using Xunit;

namespace Thinktecture.Extensions
{
	public class RegisterJsonFileConfiguration
	{
		private readonly ContainerBuilder _builder;
		private readonly Mock<IConfigurationProvider<JToken, JToken>> _providerMock;

		public RegisterJsonFileConfiguration()
		{
			_builder = new ContainerBuilder();
			_providerMock = new Mock<IConfigurationProvider<JToken, JToken>>(MockBehavior.Strict);
		}

		[Fact]
		public void Should_throw_if_builder_is_null()
		{
			((ContainerBuilder)null)
				.Invoking(b => b.RegisterJsonFileConfiguration<ConfigurationWithDefaultCtor>())
				.Should().Throw<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_on_resolution_if_config_provider_is_missing()
		{
			_builder.RegisterJsonFileConfiguration<ConfigurationWithDefaultCtor>().AsSelf();

			_builder.Build()
			        .Invoking(c => c.Resolve<ConfigurationWithDefaultCtor>())
			        .Should().Throw<DependencyResolutionException>();
		}

		[Fact]
		public void Should_delegate_config_resolution_to_configuration_provider()
		{
			var config = new ConfigurationWithDefaultCtor();
			_providerMock.Setup(p => p.GetConfiguration<ConfigurationWithDefaultCtor>(It.IsAny<IConfigurationSelector<JToken, JToken>>())).Returns(config);
			_builder.RegisterInstance(_providerMock.Object);
			_builder.RegisterJsonFileConfiguration<ConfigurationWithDefaultCtor>().AsSelf();

			_builder.Build()
			        .Resolve<ConfigurationWithDefaultCtor>()
			        .Should().Be(config);

			_providerMock.Verify(p => p.GetConfiguration<ConfigurationWithDefaultCtor>(It.IsAny<IConfigurationSelector<JToken, JToken>>()), Times.Once);
		}

		[Fact]
		public void Should_return_new_istance_even_if_configurationprovider_returned_null()
		{
			_providerMock.Setup(p => p.GetConfiguration<ConfigurationWithDefaultCtor>(It.IsAny<IConfigurationSelector<JToken, JToken>>())).Returns((ConfigurationWithDefaultCtor)null);
			_builder.RegisterInstance(_providerMock.Object);
			// ReSharper disable once RedundantArgumentDefaultValue
			_builder.RegisterJsonFileConfiguration<ConfigurationWithDefaultCtor>(null, true).AsSelf();

			_builder.Build()
			        .Resolve<ConfigurationWithDefaultCtor>()
			        .Should().NotBeNull();
		}

		[Fact]
		public void Should_raise_an_error_on_resolve_if_configurationprovider_returned_null_and_resolveNewInstanceIfNull_is_false()
		{
			_providerMock.Setup(p => p.GetConfiguration<ConfigurationWithDefaultCtor>(It.IsAny<IConfigurationSelector<JToken, JToken>>())).Returns((ConfigurationWithDefaultCtor)null);
			_builder.RegisterInstance(_providerMock.Object);
			_builder.RegisterJsonFileConfiguration<ConfigurationWithDefaultCtor>(null, false).AsSelf();

			var container = _builder.Build();

			container.Invoking(c => c.Resolve<ConfigurationWithDefaultCtor>())
			         .Should().Throw<DependencyResolutionException>()
			         .WithInnerException<DependencyResolutionException>()
			         .WithMessage($"A delegate registered to create instances of '{typeof(ConfigurationWithDefaultCtor).FullName}' returned null.");
		}
	}
}
