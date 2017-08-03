using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Microsoft.Extensions.Configuration;
using Thinktecture.Configuration;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
	/// <summary>
	/// Extensions for autofac container builder.
	/// </summary>
	public static class ContainerBuilderExtensions
	{
		internal static readonly Guid RegistrationKey = Guid.NewGuid();
		private static readonly string _registeredTypesKey = Guid.NewGuid().ToString("N");

		/// <summary>
		/// Registers <see cref="MicrosoftConfigurationLoader"/> and <see cref="MicrosoftConfigurationProvider"/>.
		/// </summary>
		/// <param name="builder">Autofac container builder.</param>
		/// <param name="configuration">An instance of <see cref="IConfiguration"/> to be used as the source.</param>
		/// <param name="converterCulture">Culture to be used by <see cref="AutofacInstanceCreator"/>. Default: <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="configuration"/> is null.</exception>
		public static void RegisterMicrosoftConfigurationProvider(this ContainerBuilder builder, IConfiguration configuration, CultureInfo converterCulture = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			builder.RegisterDefaultMicrosoftConfigurationTypes();

			builder.RegisterType<MicrosoftConfigurationLoader>()
				.WithParameter(new TypedParameter(typeof(IConfiguration), configuration))
				.As<IConfigurationLoader<IConfiguration, IConfiguration>>()
				.SingleInstance();

			builder.Register(context => context.Resolve<IConfigurationLoader<IConfiguration, IConfiguration>>().Load())
				.As<IConfigurationProvider<IConfiguration, IConfiguration>>()
				.SingleInstance();
		}

		/// <summary>
		/// Registers <see cref="MicrosoftConfigurationLoader"/> and <see cref="MicrosoftConfigurationProvider"/> with a specific registrationKey.
		/// </summary>
		/// <param name="builder">Autofac container builder.</param>
		/// <param name="configuration">An instance of <see cref="IConfiguration"/> to be used as the source.</param>
		/// <param name="converterCulture">Culture to be used by <see cref="AutofacInstanceCreator"/>. Default: <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="configuration"/> is null.</exception>
		/// <returns>A registrationKey the <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}"/> is registered with.</returns>
		public static AutofacConfigurationProviderKey RegisterKeyedMicrosoftConfigurationProvider(this ContainerBuilder builder, IConfiguration configuration, CultureInfo converterCulture = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			builder.RegisterDefaultMicrosoftConfigurationTypes();

			var key = new AutofacConfigurationProviderKey();

			builder.RegisterType<MicrosoftConfigurationLoader>()
				.WithParameter(new TypedParameter(typeof(IConfiguration), configuration))
				.Keyed<IConfigurationLoader<IConfiguration, IConfiguration>>(key)
				.SingleInstance();

			builder.Register(context => context.ResolveKeyed<IConfigurationLoader<IConfiguration, IConfiguration>>(key).Load())
				.Keyed<IConfigurationProvider<IConfiguration, IConfiguration>>(key)
				.SingleInstance();

			return key;
		}

		/// <summary>
		/// Registers default components.
		/// </summary>
		/// <param name="builder">Autofac container builder.</param>
		/// <param name="converterCulture">Culture to be used by <see cref="AutofacInstanceCreator"/>. Default: <see cref="CultureInfo.InvariantCulture"/>.</param>
		/// <returns>Instance of <see cref="ContainerBuilder"/>.</returns>
		public static ContainerBuilder RegisterDefaultMicrosoftConfigurationTypes(this ContainerBuilder builder, CultureInfo converterCulture = null)
		{
			builder.RegisterType<MicrosoftConfigurationConverter>()
				.As<IMicrosoftConfigurationConverter>()
				.IfNotRegistered(typeof(IMicrosoftConfigurationConverter));

			builder.RegisterType<AutofacInstanceCreator>()
				.As<IInstanceCreator>()
				.WithParameter(new TypedParameter(typeof(CultureInfo), converterCulture ?? CultureInfo.InvariantCulture))
				.IfNotRegistered(typeof(IInstanceCreator));

			if (!builder.IsTypeRegistered(typeof(List<>)))
			{
				builder.RegisterGeneric(typeof(List<>))
					.Keyed(RegistrationKey, typeof(ICollection<>))
					.Keyed(RegistrationKey, typeof(IEnumerable<>))
					.Keyed(RegistrationKey, typeof(List<>));
			}

			if (!builder.IsTypeRegistered(typeof(Collection<>)))
			{
				builder.RegisterGeneric(typeof(Collection<>))
					.Keyed(RegistrationKey, typeof(Collection<>));
			}

			if (!builder.IsTypeRegistered(typeof(Dictionary<,>)))
			{
				builder.RegisterGeneric(typeof(Dictionary<,>))
					.Keyed(RegistrationKey, typeof(Dictionary<,>))
					.Keyed(RegistrationKey, typeof(IDictionary<,>))
					.Keyed(RegistrationKey, typeof(IReadOnlyDictionary<,>));
			}

			return builder;
		}

		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="key">The key of the configuration section.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> is null.</exception>
		/// <returns>Autofac registration builder.</returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterMicrosoftConfiguration<T>(this ContainerBuilder builder, string key = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			var selector = String.IsNullOrWhiteSpace(key) ? null : new MicrosoftConfigurationSelector(key.Trim());
			builder.TryRegisterTypeOnce<T>();

			return builder.Register(context => context.Resolve<IConfigurationProvider<IConfiguration, IConfiguration>>().GetConfiguration<T>(selector));
		}

		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="registrationKey">The key of a <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}"/>.</param>
		/// <param name="key">The key of the configuration section.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="registrationKey"/> is null.</exception>
		/// <returns>Autofac registration builder.</returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterMicrosoftConfiguration<T>(this ContainerBuilder builder, AutofacConfigurationProviderKey registrationKey, string key = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (registrationKey == null)
				throw new ArgumentNullException(nameof(registrationKey));

			var selector = String.IsNullOrWhiteSpace(key) ? null : new MicrosoftConfigurationSelector(key.Trim());
			builder.TryRegisterTypeOnce<T>();

			return builder.Register(context => context.ResolveKeyed<IConfigurationProvider<IConfiguration, IConfiguration>>(registrationKey).GetConfiguration<T>(selector));
		}

		/// <summary>
		/// Registers type so that it can be resolved by Autofac.
		/// </summary>
		/// <typeparam name="T">Type to make resolvable.</typeparam>
		/// <param name="builder">Container builder to register the type with.</param>
		public static void RegisterMicrosoftConfigurationType<T>(this ContainerBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.TryRegisterTypeOnce<T>();
		}

		/// <summary>
		/// Registers type so that it can be resolved by Autofac.
		/// </summary>
		/// <typeparam name="TImplementation">Type to use when the type <typeparamref name="TAbstraction"/> is required.</typeparam>
		/// <typeparam name="TAbstraction">Type to make resolvable.</typeparam>
		/// <param name="builder">Container builder to register the type with.</param>
		public static void RegisterMicrosoftConfigurationType<TImplementation, TAbstraction>(this ContainerBuilder builder)
			where TImplementation : TAbstraction
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.RegisterType<TImplementation>().Keyed<TAbstraction>(RegistrationKey);
		}

		private static void TryRegisterTypeOnce<T>(this ContainerBuilder builder, bool includeInterfaces = true)
		{
			if (builder.IsTypeRegistered(typeof(T)))
				return;

			var registration = builder.RegisterType<T>().Keyed<T>(RegistrationKey);

			if (includeInterfaces)
			{
				foreach (var implementedInterface in typeof(T).GetTypeInfo().ImplementedInterfaces)
				{
					registration.Keyed(RegistrationKey, implementedInterface);
				}
			}
		}

		private static bool IsTypeRegistered(this ContainerBuilder builder, Type type)
		{
			var types = GetRegisteredTypes(builder);
			return types.Contains(type);
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