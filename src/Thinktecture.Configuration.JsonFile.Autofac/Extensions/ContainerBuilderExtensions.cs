using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using JetBrains.Annotations;
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
		public static void RegisterJsonFileConfigurationProvider([NotNull] this ContainerBuilder builder, [NotNull, ItemNotNull] params string[] configurationFilePaths)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configurationFilePaths == null)
				throw new ArgumentNullException(nameof(configurationFilePaths));
			if (configurationFilePaths.Length == 0)
				throw new ArgumentException("The number of configuration file paths cannot 0.");
			if (configurationFilePaths.Any(path => String.IsNullOrWhiteSpace(path)))
				throw new ArgumentException("At least one of the configuration file path is empty.", nameof(configurationFilePaths));

			RegisterConverterOnce(builder);
			builder.Register(context => new JsonFileConfigurationLoader(context.Resolve<IFile>(), context.Resolve<IJsonTokenConverter>(), configurationFilePaths))
					.As<IConfigurationLoader<JToken, JToken>>()
					.SingleInstance();
			builder.Register(context => context.Resolve<IConfigurationLoader<JToken, JToken>>().Load())
					.As<IConfigurationProvider<JToken, JToken>>()
					.SingleInstance();
		}

		/// <summary>
		/// Registers <see cref="JsonFileConfigurationLoader"/> and <see cref="JsonFileConfigurationProvider"/> with a specific registrationKey.
		/// </summary>
		/// <param name="builder">Autofac container builder.</param>
		/// <param name="configurationFilePaths">Paths to the configuration files. The first file is considred to be the main file, the others are overrides.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="configurationFilePaths"/> is null.</exception>
		/// <exception cref="ArgumentException">Is thrown if the <paramref name="configurationFilePaths"/> is an empty string.</exception>
		/// <returns>A registrationKey the <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}"/> is registered with.</returns>
		[NotNull]
		public static AutofacConfigurationProviderKey RegisterKeyedJsonFileConfigurationProvider([NotNull] this ContainerBuilder builder, [ItemNotNull, NotNull] params string[] configurationFilePaths)
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
					.Keyed<IConfigurationLoader<JToken, JToken>>(key)
					.SingleInstance();
			builder.Register(context => context.ResolveKeyed<IConfigurationLoader<JToken, JToken>>(key).Load())
					.Keyed<IConfigurationProvider<JToken, JToken>>(key)
					.SingleInstance();

			return key;
		}

		private static void RegisterConverterOnce([NotNull] ContainerBuilder builder)
		{
			if (builder.Properties.ContainsKey(_converterKey))
				return;

			builder.Properties[_converterKey] = true;
			builder.RegisterType<AutofacJsonTokenConverter>().As<IJsonTokenConverter>().SingleInstance();
		}

		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="propertyName">Tne name of the json property to use to build the configuration.</param>
		/// <param name="resolveNewInstanceIfNull">If <c>true</c> then a new instance of <typeparamref name="T"/> is returned even if the configuration is missing or is <c>null</c>; otherwise Autofac will raise an error.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> is null.</exception>
		/// <returns>Autofac registration builder.</returns>
		[NotNull]
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterJsonFileConfiguration<T>([NotNull] this ContainerBuilder builder, [CanBeNull] string propertyName = null, bool resolveNewInstanceIfNull = true)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			var selector = String.IsNullOrWhiteSpace(propertyName) ? null : new JsonFileConfigurationSelector(propertyName.Trim());
			RegisterTypeOnce<T>(builder);

			return builder.Register(context =>
			{
				var config = context.Resolve<IConfigurationProvider<JToken, JToken>>().GetConfiguration<T>(selector);
				return ReferenceEquals(config, null) && resolveNewInstanceIfNull ? context.ResolveConfigurationType<T>() : config;
			});
		}

		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="registrationKey">The key of a <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}"/>.</param>
		/// <param name="propertyName">Tne name of the json property to use to build the configuration.</param>
		/// <param name="resolveNewInstanceIfNull">If <c>true</c> then a new instance of <typeparamref name="T"/> is returned even if the configuration is missing or is <c>null</c>; otherwise Autofac will raise an error.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="registrationKey"/> is null.</exception>
		/// <returns>Autofac registration builder.</returns>
		[NotNull]
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterJsonFileConfiguration<T>([NotNull] this ContainerBuilder builder, [NotNull] AutofacConfigurationProviderKey registrationKey, [CanBeNull] string propertyName = null, bool resolveNewInstanceIfNull = true)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (registrationKey == null)
				throw new ArgumentNullException(nameof(registrationKey));

			var selector = String.IsNullOrWhiteSpace(propertyName) ? null : new JsonFileConfigurationSelector(propertyName.Trim());
			RegisterTypeOnce<T>(builder);

			return builder.Register(context =>
			{
				var config = context.ResolveKeyed<IConfigurationProvider<JToken, JToken>>(registrationKey).GetConfiguration<T>(selector);
				return ReferenceEquals(config, null) && resolveNewInstanceIfNull ? context.ResolveConfigurationType<T>() : config;
			});
		}

		/// <summary>
		/// Registers type so that the <see cref="AutofacJsonTokenConverter"/> is able to create instances of <typeparamref name="T"/> and all its interfaces.
		/// </summary>
		/// <typeparam name="T">Type to register.</typeparam>
		/// <param name="builder">Container builder the type to register with.</param>
		public static void RegisterJsonFileConfigurationType<T>([NotNull] this ContainerBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			RegisterTypeOnce<T>(builder);
		}

		/// <summary>
		/// Registers type so that the <see cref="AutofacJsonTokenConverter"/> is able to create instances of <typeparamref name="TAbstraction"/>.
		/// </summary>
		/// <typeparam name="TImplementation">Type to instantiate if <typeparamref name="TAbstraction"/> is required.</typeparam>
		/// <typeparam name="TAbstraction">Type to make resolvable.</typeparam>
		/// <param name="builder">Contianer builder to register the type with.</param>
		public static void RegisterJsonFileConfigurationType<TImplementation, TAbstraction>([NotNull] this ContainerBuilder builder)
			where TImplementation : TAbstraction
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			RegisterType<TImplementation, TAbstraction>(builder);
		}

		private static void RegisterTypeOnce<T>([NotNull] ContainerBuilder builder)
		{
			var types = GetRegisteredTypes(builder);

			if (types.Contains(typeof(T)))
				return;

			var registration = builder.RegisterType<T, T>();

			// do the same with all implemented interfaces.
			foreach (var implementedInterface in typeof(T).GetTypeInfo().ImplementedInterfaces)
			{
				builder.Register(context => new AutofacJsonTokenConverterType(implementedInterface)).AsSelf().SingleInstance();
				registration.Keyed(ConfigurationRegistrationKey, implementedInterface);
			}
		}

		[NotNull]
		private static IRegistrationBuilder<TImplementation, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<TImplementation, TAbstraction>([NotNull] this ContainerBuilder builder)
			where TImplementation : TAbstraction
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			// AutofacJsonTokenConverter must create a converter for T and its interfaces
			builder.Register(context => new AutofacJsonTokenConverterType(typeof(TAbstraction))).AsSelf().SingleInstance();

			// needed so that autofac is able to create instances of T and its interfaces.
			return builder.RegisterType<TImplementation>().Keyed<TAbstraction>(ConfigurationRegistrationKey);
		}

		[NotNull]
		private static HashSet<Type> GetRegisteredTypes([NotNull] ContainerBuilder builder)
		{
			HashSet<Type> types;
			if (builder.Properties.TryGetValue(_registeredTypesKey, out var value))
			{
				types = (HashSet<Type>)value;
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
