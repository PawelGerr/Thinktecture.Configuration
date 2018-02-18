using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Reads configuration from <see cref="IConfiguration"/>.
	/// </summary>
	public class MicrosoftConfigurationProvider : IConfigurationProvider<IConfiguration, IConfiguration>
	{
		private readonly IMicrosoftConfigurationConverter _converter;
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of <see cref="MicrosoftConfigurationProvider"/>.
		/// </summary>
		/// <param name="configuration">The source to be used during deserialization of the configurations.</param>
		/// <param name="converter">Reads from <paramref name="configuration"/> and populates the object.</param>
		public MicrosoftConfigurationProvider([NotNull] IConfiguration configuration, [NotNull] IMicrosoftConfigurationConverter converter)
		{
			_converter = converter ?? throw new ArgumentNullException(nameof(converter));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <inheritdoc />
		public TConfiguration GetConfiguration<TConfiguration>(IConfigurationSelector<IConfiguration, IConfiguration> selector = null)
		{
			var configuration = selector == null ? _configuration : selector.Select(_configuration);

			return _converter.Convert<TConfiguration>(configuration);
		}
	}
}
