using System;
using Autofac;
using Autofac.Builder;
using Newtonsoft.Json.Linq;
using Thinktecture.Configuration;

namespace Thinktecture.Extensions
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
		/// <param name="builder">Instance of a container builder.</param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterConfiguration<T>(this ContainerBuilder builder, string propertyName = null)
			where T : IConfiguration
		{
			if(builder == null)
				throw new ArgumentNullException(nameof(builder));

			var selector = propertyName == null ? null : new JsonFileConfigurationSelector(propertyName);

			return builder.Register(context =>
			{
				var config = context.Resolve<IConfigurationProvider<JToken>>().GetConfiguration<T>(selector);

				if(config == null)
					throw new ConfigurationNotFoundExeption(typeof(T), propertyName);

				return config;
			});
		}
	}
}