using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Converts json tokens to configurations.
	/// </summary>
	public interface IJsonTokenConverter
	{
		/// <summary>
		/// Converts provided <paramref name="token"/> to instance of <typeparamref name="TConfiguration"/>.
		/// </summary>
		/// <typeparam name="TConfiguration">Type of the configuration.</typeparam>
		/// <param name="token">Token to convert.</param>
		/// <returns>An instance of <typeparamref name="TConfiguration"/>.</returns>
		TConfiguration Convert<TConfiguration>(JToken token)
			where TConfiguration : IConfiguration;
	}
}