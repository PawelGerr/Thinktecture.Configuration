using System;
using Autofac;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
	internal static class ComponentContextExtensions
	{
		public static T ResolveConfigurationType<T>([NotNull] this IComponentContext container)
		{
			if (container == null)
				throw new ArgumentNullException(nameof(container));

			return (T)container.ResolveConfigurationType(typeof(T));
		}

		public static object ResolveConfigurationType([NotNull] this IComponentContext container, [NotNull] Type objectType)
		{
			if (container == null)
				throw new ArgumentNullException(nameof(container));
			if (objectType == null)
				throw new ArgumentNullException(nameof(objectType));

			return container.IsRegisteredWithKey(ContainerBuilderExtensions.ConfigurationRegistrationKey, objectType)
				? container.ResolveKeyed(ContainerBuilderExtensions.ConfigurationRegistrationKey, objectType)
				: container.Resolve(objectType);
		}
	}
}
