using System;
using Microsoft.Extensions.Configuration;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Selects a section.
	/// </summary>
	public class MicrosoftConfigurationSelector: IConfigurationSelector<IConfiguration, IConfiguration>
	{
		private readonly string _key;

		/// <summary>
		/// Initializes a new instance of <see cref="MicrosoftConfigurationSelector"/>.
		/// </summary>
		/// <param name="key">The key of the configuration section.</param>
		public MicrosoftConfigurationSelector(string key)
		{
			_key = key ?? throw new ArgumentNullException(nameof(key));
		}

		/// <inheritdoc />
		public IConfiguration Select(IConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			return configuration.GetSection(_key);
		}
	}
}