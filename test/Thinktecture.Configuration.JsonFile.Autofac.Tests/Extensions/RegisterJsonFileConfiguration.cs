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
		private readonly Mock<IConfigurationProvider<JToken>> _providerMock;

		public RegisterJsonFileConfiguration()
		{
			_builder = new ContainerBuilder();
			_providerMock = new Mock<IConfigurationProvider<JToken>>(MockBehavior.Strict);
		}

		[Fact]
		public void Should_throw_if_builder_is_null()
		{
			((ContainerBuilder)null)
				.Invoking(b => b.RegisterJsonFileConfiguration<ConfigurationWithDefaultCtor>())
				.ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_on_resolution_if_config_provider_is_missing()
		{
			_builder.RegisterJsonFileConfiguration<ConfigurationWithDefaultCtor>().AsSelf();

			_builder.Build()
				.Invoking(c => c.Resolve<ConfigurationWithDefaultCtor>())
				.ShouldThrow<DependencyResolutionException>();
		}

		[Fact]
		public void Should_delegate_config_resolution_to_configuration_provider()
		{
			var config = new ConfigurationWithDefaultCtor();
			_providerMock.Setup(p => p.GetConfiguration<ConfigurationWithDefaultCtor>(It.IsAny<IConfigurationSelector<JToken>>())).Returns(config);
			_builder.RegisterInstance(_providerMock.Object);
			_builder.RegisterJsonFileConfiguration<ConfigurationWithDefaultCtor>().AsSelf();

			_builder.Build()
				.Resolve<ConfigurationWithDefaultCtor>()
				.Should().Be(config);

			_providerMock.Verify(p => p.GetConfiguration<ConfigurationWithDefaultCtor>(It.IsAny<IConfigurationSelector<JToken>>()), Times.Once);
		}
	}
}