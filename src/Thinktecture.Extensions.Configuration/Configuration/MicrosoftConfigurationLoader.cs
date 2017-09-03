using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Uses <see cref="IConfiguration"/> as the source during deserialization of the configurations.
	/// </summary>
	public class MicrosoftConfigurationLoader : IConfigurationLoader<IConfiguration, IConfiguration>
	{
		[NotNull]
		private readonly Func<IConfiguration, MicrosoftConfigurationProvider> _providerFactory;
		[NotNull]
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of <see cref="MicrosoftConfigurationLoader"/>.
		/// </summary>
		/// <param name="configuration">The source to be used during deserialization of the configurations.</param>
		/// <param name="providerFactory">Factory for creation of <see cref="IMicrosoftConfigurationConverter"/>.</param>
		public MicrosoftConfigurationLoader([NotNull] IConfiguration configuration,
			[NotNull] Func<IConfiguration, MicrosoftConfigurationProvider> providerFactory)
		{
			_providerFactory = providerFactory ?? throw new ArgumentNullException(nameof(providerFactory));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <inheritdoc />
		public IConfigurationProvider<IConfiguration, IConfiguration> Load()
		{
			return _providerFactory(_configuration);
		}
	}
}
