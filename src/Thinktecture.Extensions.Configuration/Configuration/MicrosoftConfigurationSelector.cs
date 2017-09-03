using System;
using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Selects a section.
	/// </summary>
	public class MicrosoftConfigurationSelector : IConfigurationSelector<IConfiguration, IConfiguration>
	{
		private readonly string _key;

		/// <summary>
		/// Initializes a new instance of <see cref="MicrosoftConfigurationSelector"/>.
		/// </summary>
		/// <param name="key">The key of the configuration section.</param>
		public MicrosoftConfigurationSelector([NotNull] string key)
		{
			_key = key ?? throw new ArgumentNullException(nameof(key));
		}

		/// <inheritdoc />
		[NotNull]
		public IConfiguration Select([NotNull] IConfiguration configuration)
		{
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));

			return configuration.GetSection(_key);
		}
	}
}
