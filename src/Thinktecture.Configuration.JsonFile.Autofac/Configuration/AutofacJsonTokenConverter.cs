using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
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
		public AutofacJsonTokenConverter(ILifetimeScope scope, IEnumerable<AutofacJsonTokenConverterType> typesToConvertViaAutofac, IAutofacJsonTokenConverterJsonSettingsProvider jsonSerializerSettingsProvider = null)
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

			var mainToken = tokens[0];
			var mainConfig = (mainToken == null) ? default(TConfiguration) : mainToken.ToObject<TConfiguration>(serializer);

			if (mainConfig != null)
			{
				for (var i = 1; i < tokens.Length; i++)
				{
					using (var reader = new JTokenReader(tokens[i]))
					{
						serializer.Populate(reader, mainConfig);
					}
				}
			}

			return mainConfig;
		}
	}
}