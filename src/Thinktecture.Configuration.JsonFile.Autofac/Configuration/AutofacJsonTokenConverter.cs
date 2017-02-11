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
		private readonly Func<Type, JsonSerializerSettings> _jsonSerializerSettingsProvider;
		private readonly List<AutofacCreationJsonConverter> _converters;

		/// <summary>
		/// Creates new instance of <see cref="AutofacJsonTokenConverter"/>.
		/// </summary>
		/// <param name="scope">Autofac container.</param>
		/// <param name="typesToConvertViaAutofac">Types that should be converted using autofac.</param>
		/// <param name="jsonSerializerSettingsProvider">Provides <see cref="JsonSerializerSettings"/> to be used by <see cref="JsonSerializer"/>.</param>
		public AutofacJsonTokenConverter(ILifetimeScope scope, IEnumerable<Type> typesToConvertViaAutofac, Func<Type, JsonSerializerSettings> jsonSerializerSettingsProvider = null)
		{
			if (scope == null)
				throw new ArgumentNullException(nameof(scope));
			if (typesToConvertViaAutofac == null)
				throw new ArgumentNullException(nameof(typesToConvertViaAutofac));

			_jsonSerializerSettingsProvider = jsonSerializerSettingsProvider;
			_converters = typesToConvertViaAutofac.Select(t => new AutofacCreationJsonConverter(t, scope)).ToList();
		}

		/// <inheritdoc />
		public TConfiguration Convert<TConfiguration>(JToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			var serializer = (_jsonSerializerSettingsProvider == null)
				? JsonSerializer.CreateDefault()
				: JsonSerializer.CreateDefault(_jsonSerializerSettingsProvider(typeof(TConfiguration)));

			foreach (var converter in _converters)
			{
				serializer.Converters.Add(converter);
			}

			return token.ToObject<TConfiguration>(serializer);
		}
	}
}