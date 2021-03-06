using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Converts json tokens to configurations.
	/// </summary>
	public class AutofacJsonTokenConverter : IJsonTokenConverter
	{
		private readonly IAutofacJsonTokenConverterJsonSettingsProvider _jsonSerializerSettingsProvider;
		private readonly List<AutofacCreationJsonConverter> _converters;

		/// <summary>
		/// Creates new instance of <see cref="AutofacJsonTokenConverter"/>.
		/// </summary>
		/// <param name="scope">Autofac container.</param>
		/// <param name="typesToConvertViaAutofac">Types that should be converted using autofac.</param>
		/// <param name="jsonSerializerSettingsProvider">Provides <see cref="JsonSerializerSettings"/> to be used by <see cref="JsonSerializer"/>.</param>
		public AutofacJsonTokenConverter([NotNull] ILifetimeScope scope, [NotNull] IEnumerable<AutofacJsonTokenConverterType> typesToConvertViaAutofac, [CanBeNull] IAutofacJsonTokenConverterJsonSettingsProvider jsonSerializerSettingsProvider = null)
		{
			if (scope == null)
				throw new ArgumentNullException(nameof(scope));
			if (typesToConvertViaAutofac == null)
				throw new ArgumentNullException(nameof(typesToConvertViaAutofac));

			_jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
			_converters = typesToConvertViaAutofac.Select(t => new AutofacCreationJsonConverter(t.Type, scope)).ToList();
		}

		/// <inheritdoc />
		public TConfiguration Convert<TConfiguration>(JToken[] tokens)
		{
			if (tokens == null)
				throw new ArgumentNullException(nameof(tokens));
			if (tokens.Length == 0)
				throw new ArgumentException("The token collection must contains at least 1 token.");

			var serializer = (_jsonSerializerSettingsProvider == null)
				? JsonSerializer.CreateDefault()
				: JsonSerializer.CreateDefault(_jsonSerializerSettingsProvider.GetSettings(typeof(TConfiguration)));

			foreach (var converter in _converters)
			{
				serializer.Converters.Add(converter);
			}

			var lastToken = tokens.LastOrDefault(t => t != null);

			if (IsNull(lastToken))
				return default;

			var startToken = tokens.Skip(GetStartIndex(tokens)).First(t => t != null && t.Type != JTokenType.Null);
			var config = startToken.ToObject<TConfiguration>(serializer);

			if (ReferenceEquals(config, null))
				return default;

         var startIndex = Array.IndexOf(tokens, startToken);

			for (var i = startIndex + 1; i < tokens.Length; i++)
			{
				var token = tokens[i];

				if (!IsNull(token))
				{
					using (var reader = new JTokenReader(token))
					{
						serializer.Populate(reader, config);
					}
				}
			}

			return config;
		}

		private static int GetStartIndex([NotNull] JToken[] tokens)
		{
			if (tokens == null)
				throw new ArgumentNullException(nameof(tokens));

			for (var i = tokens.Length - 1; i >= 0; i--)
			{
				var token = tokens[i];

				if (token?.Type == JTokenType.Null)
					return i + 1;
			}

			return 0;
		}

		private static bool IsNull([CanBeNull] JToken token)
		{
			return token == null || token.Type == JTokenType.Null;
		}
	}
}
