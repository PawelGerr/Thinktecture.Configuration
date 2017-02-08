namespace Thinktecture.Configuration
{
	/// <summary>
	/// Loads configuration.
	/// </summary>
	/// <typeparam name="TRawData">Type of the data the <see cref="IConfigurationProvider{T}"/> works on.</typeparam>
	public interface IConfigurationLoader<TRawData>
	{
		/// <summary>
		/// Loads configuration.
		/// </summary>
		/// <returns>Configuration provider.</returns>
		IConfigurationProvider<TRawData> Load();
	}
}