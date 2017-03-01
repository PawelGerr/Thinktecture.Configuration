namespace Thinktecture.Configuration
{
	/// <summary>
	/// Loads configuration.
	/// </summary>
	/// <typeparam name="TRawDataIn">Type of the data the <see cref="IConfigurationSelector{TRawDataIn,TRawDataOut}"/> selects from.</typeparam>
	/// <typeparam name="TRawDataOut">Type of the data the <see cref="IConfigurationSelector{TRawDataIn,TRawDataOut}"/> returns.</typeparam>
	public interface IConfigurationLoader<out TRawDataIn, in TRawDataOut>
	{
		/// <summary>
		/// Loads configuration.
		/// </summary>
		/// <returns>Configuration provider.</returns>
		IConfigurationProvider<TRawDataIn, TRawDataOut> Load();
	}
}