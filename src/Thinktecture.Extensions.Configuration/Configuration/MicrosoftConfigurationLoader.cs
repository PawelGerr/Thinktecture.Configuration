using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Uses <see cref="IConfiguration"/> as the source during deserialization of the configurations.
	/// </summary>
	public class MicrosoftConfigurationLoader : IConfigurationLoader<IConfiguration, IConfiguration>
	{
		private readonly Func<IMicrosoftConfigurationConverter> _converterFactory;
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes a new instance of <see cref="MicrosoftConfigurationLoader"/>.
		/// </summary>
		/// <param name="configuration">The source to be used during deserialization of the configurations.</param>
		/// <param name="binderFactory">Factory for creation of <see cref="IMicrosoftConfigurationConverter"/>.</param>
		public MicrosoftConfigurationLoader(IConfiguration configuration, Func<IMicrosoftConfigurationConverter> binderFactory)
		{
			_converterFactory = binderFactory ?? throw new ArgumentNullException(nameof(binderFactory));
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <inheritdoc />
		public IConfigurationProvider<IConfiguration, IConfiguration> Load()
		{
			return new MicrosoftConfigurationProvider(_configuration, _converterFactory());
		}
	}
}
