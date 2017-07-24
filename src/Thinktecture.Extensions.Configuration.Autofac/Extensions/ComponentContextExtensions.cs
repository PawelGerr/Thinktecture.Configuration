using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
	internal static class ComponentContextExtensions
	{
		public static T ResolveConfigurationType<T>(this IComponentContext container)
		{
			return (T) container.ResolveConfigurationType(typeof(T));
		}

		public static object ResolveConfigurationType(this IComponentContext container, Type objectType)
		{
			return container.IsRegisteredWithKey(ContainerBuilderExtensions.RegistrationKey, objectType)
				? container.ResolveKeyed(ContainerBuilderExtensions.RegistrationKey, objectType)
				: container.Resolve(objectType);
		}
	}
}