using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Cache for configurations using <see cref="IConfiguration"/> as the source.
	/// </summary>
	/// <typeparam name="T">Type of the configuration.</typeparam>
	public class MicrosoftConfigurationCache<T> : IConfigurationCache<T>, IDisposable
	{
		private readonly IConfigurationProvider<IConfiguration, IConfiguration> _configurationProvider;
		private readonly IConfigurationSelector<IConfiguration, IConfiguration> _selector;
		private readonly IDisposable _changeTokenListener;

		private Lazy<T> _lazyConfig;

		/// <inheritdoc />
		public T CurrentValue => _lazyConfig.Value;

		/// <summary>
		/// Initializes new instance of <see cref="MicrosoftConfigurationCache{T}"/>.
		/// </summary>
		/// <param name="configurationProvider">Configuration provider.</param>
		/// <param name="selector">Configuration selector.</param>
		/// <param name="changeTokenSource">Change token.</param>
		public MicrosoftConfigurationCache(
			IConfigurationProvider<IConfiguration, IConfiguration> configurationProvider,
			IConfigurationSelector<IConfiguration, IConfiguration> selector,
			MicrosoftConfigurationChangeTokenSource changeTokenSource)
		{
			if (changeTokenSource == null)
				throw new ArgumentNullException(nameof(changeTokenSource));

			_configurationProvider = configurationProvider ?? throw new ArgumentNullException(nameof(configurationProvider));
			_selector = selector;

			_changeTokenListener = ChangeToken.OnChange(changeTokenSource.GetChangeToken, () => _lazyConfig = CreateLazyConfig());
			_lazyConfig = CreateLazyConfig();
		}

		private Lazy<T> CreateLazyConfig()
		{
			return new Lazy<T>(() => _configurationProvider.GetConfiguration<T>(_selector));
		}

		/// <inheritdoc />
		public void Dispose()
		{
			_changeTokenListener.Dispose();
		}
	}
}