using System;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Provides configurations.
	/// </summary>
	/// <typeparam name="TRawData">Type of the data the <see cref="IConfigurationProvider{T}"/> works on.</typeparam>
	public interface IConfigurationProvider<TRawData>
	{
		/// <summary>
		/// Gets the value as a type <typeparam name="TConfiguration">TConfiguration</typeparam>
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		TConfiguration GetConfiguration<TConfiguration>(IConfigurationSelector<TRawData> selector = null);
	}
}