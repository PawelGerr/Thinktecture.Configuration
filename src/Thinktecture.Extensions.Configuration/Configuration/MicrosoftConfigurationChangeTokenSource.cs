using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Wrapper for the real <see cref="IChangeToken"/>.
	/// </summary>
	public class MicrosoftConfigurationChangeTokenSource
	{
		private readonly IConfiguration _configuration;

		/// <summary>
		/// Initializes new instance of <see cref="MicrosoftConfigurationChangeTokenSource"/>.
		/// </summary>
		/// <param name="configuration">Configuration.</param>
		public MicrosoftConfigurationChangeTokenSource(IConfiguration configuration)
		{
			_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
		}

		/// <summary>
		/// Gets change token of the configuration.
		/// </summary>
		/// <returns>Change token of the configuration.</returns>
		public IChangeToken GetChangeToken()
		{
			return _configuration.GetReloadToken();
		}
	}
}