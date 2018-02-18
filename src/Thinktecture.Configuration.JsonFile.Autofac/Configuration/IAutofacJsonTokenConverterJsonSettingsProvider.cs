using System;
using Newtonsoft.Json;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Provides <see cref="JsonSerializerSettings"/> to be used by <see cref="AutofacJsonTokenConverter"/>.
	/// </summary>
	public interface IAutofacJsonTokenConverterJsonSettingsProvider
	{
		/// <summary>
		/// Gets <see cref="JsonSerializerSettings"/> for provided <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Type to deserialize.</param>
		/// <returns>An instance of <see cref="JsonSerializerSettings"/>.</returns>
		JsonSerializerSettings GetSettings(Type type);
	}
}
