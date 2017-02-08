namespace Thinktecture.Configuration
{
	/// <summary>
	/// Selects the configuration.
	/// </summary>
	/// <typeparam name="TRawData">Type of the data the <see cref="IConfigurationProvider{T}"/> works on.</typeparam>
	public interface IConfigurationSelector<TRawData>
	{
		/// <summary>
		/// Selects the data to build configuration from.
		/// </summary>
		/// <param name="rawData">Data to extract the correct part from.</param>
		/// <returns>Configuration data.</returns>
		TRawData Select(TRawData rawData);
	}
}