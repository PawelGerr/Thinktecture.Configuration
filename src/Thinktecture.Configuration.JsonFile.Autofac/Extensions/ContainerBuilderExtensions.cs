using System;
using Autofac;
using Autofac.Builder;
using Newtonsoft.Json.Linq;
using Thinktecture.Configuration;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
	/// <summary>
	/// Extension for autofac container builder.
	/// </summary>
	public static class ContainerBuilderExtensions
	{
		/// <summary>
		/// Registeres a type that will be used with <see cref="IConfigurationProvider{TRawData}.GetConfiguration{T}"/>.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="builder">Container builder.</param>
		/// <param name="propertyName">Tne name of the json property to use to build the configuration.</param>
		/// <returns>Autofac registration builder.</returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterJsonFileConfiguration<T>(this ContainerBuilder builder, string propertyName = null)
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			var selector = String.IsNullOrWhiteSpace(propertyName) ? null : new JsonFileConfigurationSelector(propertyName.Trim());

			return builder.Register(context => context.Resolve<IConfigurationProvider<JToken>>().GetConfiguration<T>(selector));
		}
	}
}