using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using JetBrains.Annotations;
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
		public static void RegisterMicrosoftConfigurationProvider([NotNull] this ContainerBuilder builder, [NotNull] IConfiguration configuration, [CanBeNull] CultureInfo converterCulture = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			builder.RegisterDefaultMicrosoftConfigurationTypes(converterCulture);

			builder.RegisterInstance(new MicrosoftConfigurationChangeTokenSource(configuration)).AsSelf();

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
		[NotNull]
		public static AutofacConfigurationProviderKey RegisterKeyedMicrosoftConfigurationProvider([NotNull] this ContainerBuilder builder, [NotNull] IConfiguration configuration, [CanBeNull] CultureInfo converterCulture = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			builder.RegisterDefaultMicrosoftConfigurationTypes(converterCulture);

			var key = new AutofacConfigurationProviderKey();

			builder.RegisterInstance(new MicrosoftConfigurationChangeTokenSource(configuration))
			       .Keyed<MicrosoftConfigurationChangeTokenSource>(key);

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
		[NotNull]
		public static ContainerBuilder RegisterDefaultMicrosoftConfigurationTypes([NotNull] this ContainerBuilder builder, [CanBeNull] CultureInfo converterCulture = null)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.RegisterType<MicrosoftConfigurationProvider>()
			       .AsSelf()
			       .IfNotRegistered(typeof(MicrosoftConfigurationProvider));

			builder.RegisterType<MicrosoftConfigurationConverter>()
			       .As<IMicrosoftConfigurationConverter>()
			       .IfNotRegistered(typeof(IMicrosoftConfigurationConverter));

			builder.RegisterType<AutofacInstanceCreator>()
			       .As<IInstanceCreator>()
			       .WithParameter(new TypedParameter(typeof(CultureInfo), converterCulture ?? CultureInfo.InvariantCulture))
			       .IfNotRegistered(typeof(IInstanceCreator));

			if (builder.FlagTypeAsRegistered(typeof(List<>)))
			{
				builder.RegisterGeneric(typeof(List<>))
				       .Keyed(RegistrationKey, typeof(List<>))
				       .Keyed(RegistrationKey, typeof(IList<>))
				       .Keyed(RegistrationKey, typeof(ICollection<>))
				       .Keyed(RegistrationKey, typeof(IEnumerable<>));
			}

			if (builder.FlagTypeAsRegistered(typeof(Collection<>)))
			{
				builder.RegisterGeneric(typeof(Collection<>))
				       .Keyed(RegistrationKey, typeof(Collection<>));
			}

			if (builder.FlagTypeAsRegistered(typeof(Dictionary<,>)))
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
		/// <param name="reuseInstance">If <c>true</c> then the configuration of type <typeparamref name="T"/> will be reused until the unterlying <see cref="IConfiguration"/> has been changed.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> is null.</exception>
		/// <returns>Autofac registration builder.</returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterMicrosoftConfiguration<T>([NotNull] this ContainerBuilder builder, [CanBeNull] string key = null, bool reuseInstance = true)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			// ReSharper disable once PossibleNullReferenceException
			var selector = String.IsNullOrWhiteSpace(key) ? null : new MicrosoftConfigurationSelector(key.Trim());
			builder.TryRegisterTypeOnce<T>();

			if (reuseInstance)
			{
				builder.RegisterType<MicrosoftConfigurationCache<T>>()
				       .WithParameter(new TypedParameter(typeof(IConfigurationSelector<IConfiguration, IConfiguration>), selector))
				       .As<IConfigurationCache<T>>()
				       .SingleInstance();

				return builder.Register(context => context.Resolve<IConfigurationCache<T>>().CurrentValue);
			}

			return builder.Register(context => context.Resolve<IConfigurationProvider<IConfiguration, IConfiguration>>().GetConfiguration<T>(selector));
		}

		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="registrationKey">The key of a <see cref="IConfigurationProvider{TRawDataIn,TRawDataOut}"/>.</param>
		/// <param name="key">The key of the configuration section.</param>
		/// <param name="reuseInstance">If <c>true</c> then the configuration of type <typeparamref name="T"/> will be reused until the unterlying <see cref="IConfiguration"/> has been changed.</param>
		/// <exception cref="ArgumentNullException">Is thrown if the <paramref name="builder"/> or the <paramref name="registrationKey"/> is null.</exception>
		/// <returns>Autofac registration builder.</returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterMicrosoftConfiguration<T>([NotNull] this ContainerBuilder builder, [NotNull] AutofacConfigurationProviderKey registrationKey, [CanBeNull] string key = null, bool reuseInstance = true)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (registrationKey == null)
				throw new ArgumentNullException(nameof(registrationKey));

			// ReSharper disable once PossibleNullReferenceException
			var selector = String.IsNullOrWhiteSpace(key) ? null : new MicrosoftConfigurationSelector(key.Trim());
			builder.TryRegisterTypeOnce<T>();

			if (reuseInstance)
			{
				builder.RegisterType<MicrosoftConfigurationCache<T>>()
				       .WithParameter(new ResolvedParameter((info, context) => info.ParameterType == typeof(IConfigurationProvider<IConfiguration, IConfiguration>), (info, context) => context.ResolveKeyed<IConfigurationProvider<IConfiguration, IConfiguration>>(registrationKey)))
				       .WithParameter(new TypedParameter(typeof(IConfigurationSelector<IConfiguration, IConfiguration>), selector))
				       .WithParameter(new ResolvedParameter((info, context) => info.ParameterType == typeof(MicrosoftConfigurationChangeTokenSource), (info, context) => context.ResolveKeyed<MicrosoftConfigurationChangeTokenSource>(registrationKey)))
				       .Keyed<IConfigurationCache<T>>(registrationKey)
				       .SingleInstance();

				return builder.Register(context => context.ResolveKeyed<IConfigurationCache<T>>(registrationKey).CurrentValue);
			}

			return builder.Register(context => context.ResolveKeyed<IConfigurationProvider<IConfiguration, IConfiguration>>(registrationKey).GetConfiguration<T>(selector));
		}

		/// <summary>
		/// Registers type so that it can be resolved by Autofac.
		/// </summary>
		/// <typeparam name="T">Type to make resolvable.</typeparam>
		/// <param name="builder">Container builder to register the type with.</param>
		public static void RegisterMicrosoftConfigurationType<T>([NotNull] this ContainerBuilder builder)
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
		public static void RegisterMicrosoftConfigurationType<TImplementation, TAbstraction>([NotNull] this ContainerBuilder builder)
			where TImplementation : TAbstraction
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			builder.RegisterType<TImplementation>().Keyed<TAbstraction>(RegistrationKey);
		}

		private static void TryRegisterTypeOnce<T>([NotNull] this ContainerBuilder builder, bool includeInterfaces = true)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

			if (!builder.FlagTypeAsRegistered(typeof(T)))
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

		/// <summary>
		/// Flags a type as registered.
		/// </summary>
		/// <returns><c>true</c> if type has been flagged as registered; <c>false</c> if the type was flagged already.</returns>
		private static bool FlagTypeAsRegistered([NotNull] this ContainerBuilder builder, [NotNull] Type type)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			var types = GetRegisteredTypes(builder);
			return types.Add(type);
		}

		private static HashSet<Type> GetRegisteredTypes([NotNull] ContainerBuilder builder)
		{
			if (builder == null)
				throw new ArgumentNullException(nameof(builder));

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
