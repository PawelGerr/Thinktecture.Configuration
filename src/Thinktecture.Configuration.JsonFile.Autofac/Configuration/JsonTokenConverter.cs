using System;
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
		private readonly ILifetimeScope _scope;

		/// <summary>
		/// Creates new instance of <see cref="AutofacJsonTokenConverter"/>.
		/// </summary>
		/// <param name="scope">Autofac container.</param>
		public AutofacJsonTokenConverter(ILifetimeScope scope)
		{
			if (scope == null)
				throw new ArgumentNullException(nameof(scope));

			_scope = scope;
		}

		/// <inheritdoc />
		public TConfiguration Convert<TConfiguration>(JToken token)
			where TConfiguration : IConfiguration
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			var serializer = JsonSerializer.CreateDefault();
			var regFound = _scope.IsRegistered(typeof(TConfiguration));

			if (!regFound)
				serializer.Converters.Add(new AutofacCreationJsonConverter(typeof(TConfiguration), _scope));

			return token.ToObject<TConfiguration>(serializer);
		}
	}
}