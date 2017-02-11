using System;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Newtonsoft.Json.Linq;
using Thinktecture.Configuration;
using Thinktecture.IO;

// ReSharper disable once CheckNamespace

namespace Thinktecture
{
	/// <summary>
	/// Extension for autofac container builder.
	/// </summary>
	public static class ContainerBuilderExtensions
	{
		internal static readonly Guid ConfigurationRegistrationKey = Guid.NewGuid();

		/// <summary>
		/// Registers <see cref="JsonFileConfigurationLoader"/> and <see cref="JsonFileConfigurationProvider"/>.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="configurationFilePath"></param>
		/// <exception cref="ArgumentNullException"></exception>
		public static void RegisterJsonFileConfigurationProvider(this ContainerBuilder builder, string configurationFilePath)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configurationFilePath == null)
				throw new ArgumentNullException(nameof(configurationFilePath));

			builder.RegisterType<AutofacJsonTokenConverter>().AsImplementedInterfaces().SingleInstance();
			builder.Register(context => new JsonFileConfigurationLoader(context.Resolve<IFile>(), configurationFilePath, context.Resolve<IJsonTokenConverter>())).AsImplementedInterfaces().SingleInstance();
			builder.Register(context => context.Resolve<IConfigurationLoader<JToken>>().Load()).AsImplementedInterfaces().SingleInstance();
		}

		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawData}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="propertyName">Tne name of the json property to use to build the configuration.</param>
		/// <returns>Autofac registration builder.</returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterJsonFileConfiguration<T>(this ContainerBuilder builder, string propertyName = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			var selector = String.IsNullOrWhiteSpace(propertyName) ? null : new JsonFileConfigurationSelector(propertyName.Trim());

			builder.RegisterJsonFileConfigurationType<T>();

			return builder.Register(context => context.Resolve<IConfigurationProvider<JToken>>().GetConfiguration<T>(selector));
		}

		/// <summary>
		/// Registers type so that the <see cref="AutofacJsonTokenConverter"/> is able to create instances of <typeparamref name="T"/> and its interfaces.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="builder"></param>
		public static void RegisterJsonFileConfigurationType<T>(this ContainerBuilder builder)
		{
			// AutofacJsonTokenConverter must create a converter for T and its interfaces
			builder.Register(context => new AutofacJsonTokenConverterType(typeof(T))).AsSelf().SingleInstance();

			// needed so that autofac is able to create instances of T and its interfaces.
			var registration = builder.RegisterType<T>().Keyed<T>(ConfigurationRegistrationKey);

			// do the same with all implemented interfaces.
			foreach (var implementedInterface in typeof(T).GetTypeInfo().ImplementedInterfaces)
			{
				builder.Register(context => new AutofacJsonTokenConverterType(implementedInterface)).AsSelf().SingleInstance();
				registration.Keyed(ConfigurationRegistrationKey, implementedInterface);
			}
		}
	}
}