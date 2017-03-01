using System;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Reads configuration from <see cref="JToken"/>.
	/// </summary>
	public class JsonFileConfigurationProvider : IConfigurationProvider<JToken, JToken>
	{
		private readonly JToken[] _tokens; // one of the token may be null
		private readonly IJsonTokenConverter _tokenConverter;

		/// <summary>
		/// Creates new instance of <see cref="JsonFileConfigurationProvider"/>.
		/// </summary>
		/// <param name="tokens">Tokens the configurations are deserialized from. The first token is considered to be the main file, the others act as overrides.</param>
		/// <param name="tokenConverter">Json deserializer.</param>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="tokens"/> or <paramref name="tokenConverter"/> is <c>null</c>.</exception>
		public JsonFileConfigurationProvider(JToken[] tokens, IJsonTokenConverter tokenConverter)
		{
			if (tokens == null)
				throw new ArgumentNullException(nameof(tokens));
			if (tokenConverter == null)
				throw new ArgumentNullException(nameof(tokenConverter));
			if (tokens.Length == 0)
				throw new ArgumentException("Token collection can not be empty.", nameof(tokens));

			_tokens = tokens;
			_tokenConverter = tokenConverter;
		}

		/// <inheritdoc />
		public TConfiguration GetConfiguration<TConfiguration>(IConfigurationSelector<JToken, JToken> selector = null)
		{
			var tokens = SelectTokens(selector);

			return _tokenConverter.Convert<TConfiguration>(tokens);
		}

		private JToken[] SelectTokens(IConfigurationSelector<JToken, JToken> selector)
		{
			if (selector == null)
				return _tokens;

			var configs = new JToken[_tokens.Length];

			for (var i = 0; i < _tokens.Length; i++)
			{
				var config = _tokens[i];

				if (config != null)
					config = selector.Select(config);

				configs[i] = config;
			}

			return configs;
		}
	}
}