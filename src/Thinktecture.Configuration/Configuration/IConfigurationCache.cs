namespace Thinktecture.Configuration
{
	/// <summary>
	/// Caches the configuration of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">Type of the configuration.</typeparam>
	public interface IConfigurationCache<out T>
	{
		/// <summary>
		/// Current cached configuration.
		/// </summary>
		T CurrentValue { get; }
	}
}
