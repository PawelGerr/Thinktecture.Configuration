using System;
using System.Collections.Generic;
using System.Linq;
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
		private static readonly string _converterKey = Guid.NewGuid().ToString("N");
		private static readonly string _registeredTypesKey = Guid.NewGuid().ToString("N");

		/// <summary>
		/// Registers <see cref="JsonFileConfigurationLoader"/> and <see cref="JsonFileConfigurationProvider"/>.
		/// </summary>
		/// <param name="builder">Autofac container builder.</param>
		/// <param name="configurationFilePaths">Paths to the configuration files. The first file is considred to be the main file, the others are overrides.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="configurationFilePaths"/> is null.</exception>
		/// <exception cref="ArgumentException">Is thrown if the <paramref name="configurationFilePaths"/> contains an empty string.</exception>
		public static void RegisterJsonFileConfigurationProvider(this ContainerBuilder builder, params string[] configurationFilePaths)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configurationFilePaths == null)
				throw new ArgumentNullException(nameof(configurationFilePaths));
			if (configurationFilePaths.Any(path => String.IsNullOrWhiteSpace(path)))
				throw new ArgumentException("At least one of the configuration file path is empty.", nameof(configurationFilePaths));

			RegisterConverterOnce(builder);
			builder.Register(context => new JsonFileConfigurationLoader(context.Resolve<IFile>(), context.Resolve<IJsonTokenConverter>(), configurationFilePaths))
				.As<IConfigurationLoader<JToken>>()
				.SingleInstance();
			builder.Register(context => context.Resolve<IConfigurationLoader<JToken>>().Load())
				.As<IConfigurationProvider<JToken>>()
				.SingleInstance();
		}

		/// <summary>
		/// Registers <see cref="JsonFileConfigurationLoader"/> and <see cref="JsonFileConfigurationProvider"/> with a specific registrationKey.
		/// </summary>
		/// <param name="builder">Autofac container builder.</param>
		/// <param name="configurationFilePaths">Paths to the configuration files. The first file is considred to be the main file, the others are overrides.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="configurationFilePaths"/> is null.</exception>
		/// <exception cref="ArgumentException">Is thrown if the <paramref name="configurationFilePaths"/> is an empty string.</exception>
		/// <returns>A registrationKey the <see cref="IConfigurationProvider{TRawData}"/> is registered with.</returns>
		public static AutofacConfigurationProviderKey RegisterKeyedJsonFileConfigurationProvider(this ContainerBuilder builder, params string[] configurationFilePaths)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configurationFilePaths == null)
				throw new ArgumentNullException(nameof(configurationFilePaths));
			if (configurationFilePaths.Any(path => String.IsNullOrWhiteSpace(path)))
				throw new ArgumentException("At least one of the configuration file path is empty.", nameof(configurationFilePaths));

			var key = new AutofacConfigurationProviderKey();
			RegisterConverterOnce(builder);
			builder.Register(context => new JsonFileConfigurationLoader(context.Resolve<IFile>(), context.Resolve<IJsonTokenConverter>(), configurationFilePaths))
				.Keyed<IConfigurationLoader<JToken>>(key)
				.SingleInstance();
			builder.Register(context => context.ResolveKeyed<IConfigurationLoader<JToken>>(key).Load())
				.Keyed<IConfigurationProvider<JToken>>(key)
				.SingleInstance();

			return key;
		}

		private static void RegisterConverterOnce(ContainerBuilder builder)
		{
			object isRegisterd;
			if (!builder.Properties.TryGetValue(_converterKey, out isRegisterd))
			{
				builder.Properties[_converterKey] = true;
				builder.RegisterType<AutofacJsonTokenConverter>().AsImplementedInterfaces().SingleInstance();
			}
		}

		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawData}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="propertyName">Tne name of the json property to use to build the configuration.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> is null.</exception>
		/// <returns>Autofac registration builder.</returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterJsonFileConfiguration<T>(this ContainerBuilder builder, string propertyName = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			var selector = String.IsNullOrWhiteSpace(propertyName) ? null : new JsonFileConfigurationSelector(propertyName.Trim());
			RegisterTypeOnce<T>(builder);

			return builder.Register(context => context.Resolve<IConfigurationProvider<JToken>>().GetConfiguration<T>(selector));
		}

		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawData}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="registrationKey">The key of a <see cref="IConfigurationProvider{TRawData}"/>.</param>
		/// <param name="propertyName">Tne name of the json property to use to build the configuration.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="registrationKey"/> is null.</exception>
		/// <returns>Autofac registration builder.</returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterJsonFileConfiguration<T>(this ContainerBuilder builder, AutofacConfigurationProviderKey registrationKey, string propertyName = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (registrationKey == null)
				throw new ArgumentNullException(nameof(registrationKey));

			var selector = String.IsNullOrWhiteSpace(propertyName) ? null : new JsonFileConfigurationSelector(propertyName.Trim());
			RegisterTypeOnce<T>(builder);

			return builder.Register(context => context.ResolveKeyed<IConfigurationProvider<JToken>>(registrationKey).GetConfiguration<T>(selector));
		}

		/// <summary>
		/// Registers type so that the <see cref="AutofacJsonTokenConverter"/> is able to create instances of <typeparamref name="T"/> and its interfaces.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="builder"></param>
		public static void RegisterJsonFileConfigurationType<T>(this ContainerBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			RegisterTypeOnce<T>(builder);
		}

		private static void RegisterTypeOnce<T>(ContainerBuilder builder)
		{
			var types = GetRegisteredTypes(builder);

			if (types.Contains(typeof(T)))
				return;

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

		private static HashSet<Type> GetRegisteredTypes(ContainerBuilder builder)
		{
			HashSet<Type> types;
			object value;
			if (builder.Properties.TryGetValue(_registeredTypesKey, out value))
			{
				types = (HashSet<Type>) value;
			}
			else
			{
				types = new HashSet<Type>();
				builder.Properties.Add(_registeredTypesKey, types);
			}

			return types;
		}
	}
}