using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Converts json tokens to configurations.
	/// </summary>
	public interface IJsonTokenConverter
	{
		/// <summary>
		/// Converts provided <paramref name="tokens"/> to instance of <typeparamref name="TConfiguration"/>.
		/// </summary>
		/// <typeparam name="TConfiguration">Type of the configuration.</typeparam>
		/// <param name="tokens">The first token is considered to be the main token, the others act as overrides.</param>
		/// <returns>An instance of <typeparamref name="TConfiguration"/>.</returns>
		TConfiguration Convert<TConfiguration>(JToken[] tokens);
	}
}
