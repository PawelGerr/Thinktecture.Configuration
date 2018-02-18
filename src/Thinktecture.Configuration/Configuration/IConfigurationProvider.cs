using JetBrains.Annotations;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Provides configurations.
	/// </summary>
	/// <typeparam name="TRawDataIn">Type of the data the <see cref="IConfigurationSelector{TRawDataIn,TRawDataOut}"/> selects from.</typeparam>
	/// <typeparam name="TRawDataOut">Type of the data the <see cref="IConfigurationSelector{TRawDataIn,TRawDataOut}"/> returns.</typeparam>
	public interface IConfigurationProvider<out TRawDataIn, in TRawDataOut>
	{
		/// <summary>
		/// Gets the value as a typed configuration.
		/// </summary>
		/// <typeparam name="TConfiguration">Type of the configuration.</typeparam>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		[CanBeNull]
		TConfiguration GetConfiguration<TConfiguration>([CanBeNull] IConfigurationSelector<TRawDataIn, TRawDataOut> selector = null);
	}
}
