using System;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Reads configuration from <see cref="JToken"/>.
	/// </summary>
	public class JsonFileConfigurationProvider : IConfigurationProvider<JToken>
	{
		private readonly JToken _token; // may be null
		private readonly IJsonTokenConverter _tokenConverter;

		/// <summary>
		/// Creates new instance of <see cref="JsonFileConfigurationProvider"/>.
		/// </summary>
		/// <param name="token">Configuration data.</param>
		/// <param name="tokenConverter">Json deserializer.</param>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="token"/> is <c>null</c>.</exception>
		public JsonFileConfigurationProvider(JToken token, IJsonTokenConverter tokenConverter)
		{
			if (tokenConverter == null)
				throw new ArgumentNullException(nameof(tokenConverter));

			_token = token;
			_tokenConverter = tokenConverter;
		}

		/// <inheritdoc />
		public TConfiguration GetConfiguration<TConfiguration>(IConfigurationSelector<JToken> selector = null)
		{
			var token = GetToken(selector);

			return (token == null) ? default(TConfiguration) : _tokenConverter.Convert<TConfiguration>(token);
		}

		private JToken GetToken(IConfigurationSelector<JToken> selector)
		{
			var config = _token;

			if (config != null && selector != null)
				config = selector.Select(config);

			return config;
		}
	}
}