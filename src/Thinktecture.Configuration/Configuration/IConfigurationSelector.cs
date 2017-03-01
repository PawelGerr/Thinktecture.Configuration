namespace Thinktecture.Configuration
{
	/// <summary>
	/// Selects the configuration.
	/// </summary>
	/// <typeparam name="TRawDataIn">Type of the data to select from.</typeparam>
	/// <typeparam name="TRawDataOut">Type of the data to return.</typeparam>
	public interface IConfigurationSelector<in TRawDataIn, out TRawDataOut>
	{
		/// <summary>
		/// Selects the data to build configuration from.
		/// </summary>
		/// <param name="rawData">Data to extract the correct part from.</param>
		/// <returns>Configuration data.</returns>
		TRawDataOut Select(TRawDataIn rawData);
	}
}