using System;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Reads configuration from <see cref="JToken"/>.
	/// </summary>
	public class JsonFileConfigurationProvider : IConfigurationProvider<JToken>
	{
		private readonly JToken _configuration;
		private readonly IJsonTokenConverter _tokenConverter;

		/// <summary>
		/// Creates new instance of <see cref="JsonFileConfigurationProvider"/>.
		/// </summary>
		/// <param name="configuration">Configuration data.</param>
		/// <param name="tokenConverter">Json deserializer.</param>
		/// <exception cref="ArgumentNullException">Thrown when the <paramref name="configuration"/> is <c>null</c>.</exception>
		public JsonFileConfigurationProvider(JToken configuration, IJsonTokenConverter tokenConverter)
		{
			if (configuration == null)
				throw new ArgumentNullException(nameof(configuration));
			if (tokenConverter == null)
				throw new ArgumentNullException(nameof(tokenConverter));

			_configuration = configuration;
			_tokenConverter = tokenConverter;
		}

		/// <inheritdoc />
		public string GetString(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<string>();
		}

		/// <inheritdoc />
		public bool? GetBool(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<bool>();
		}

		/// <inheritdoc />
		public int? GetInt32(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<int>();
		}

		/// <inheritdoc />
		public long? GetInt64(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<long>();
		}

		/// <inheritdoc />
		public DateTime? GetDateTime(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<DateTime>();
		}

		/// <inheritdoc />
		public TimeSpan? GetTimeSpan(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<TimeSpan>();
		}

		/// <inheritdoc />
		public decimal? GetDecimal(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<decimal>();
		}

		/// <inheritdoc />
		public float? GetFloat(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<float>();
		}

		/// <inheritdoc />
		public double? GetDouble(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<double>();
		}

		/// <inheritdoc />
		public Guid? GetGuid(IConfigurationSelector<JToken> selector = null)
		{
			return GetToken(selector)?.Value<Guid>();
		}

		/// <inheritdoc />
		public TConfiguration GetConfiguration<TConfiguration>(IConfigurationSelector<JToken> selector = null)
			where TConfiguration : IConfiguration
		{
			var token = GetToken(selector);

			return (token == null) ? default(TConfiguration) : _tokenConverter.Convert<TConfiguration>(token);
		}

		private JToken GetToken(IConfigurationSelector<JToken> selector)
		{
			var config = _configuration;

			if (selector != null)
				config = selector.Select(config);

			return config;
		}
	}
}