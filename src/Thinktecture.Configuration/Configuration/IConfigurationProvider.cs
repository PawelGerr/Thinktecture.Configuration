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
		/// Gets the value as a <see cref="string"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		string GetString(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="bool"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		bool? GetBool(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="int"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		int? GetInt32(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="long"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		long? GetInt64(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="DateTime"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		DateTime? GetDateTime(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="TimeSpan"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		TimeSpan? GetTimeSpan(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="decimal"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		decimal? GetDecimal(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="float"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		float? GetFloat(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="double"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		double? GetDouble(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a <see cref="Guid"/>.
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		Guid? GetGuid(IConfigurationSelector<TRawData> selector = null);

		/// <summary>
		/// Gets the value as a type <typeparam name="TConfiguration">TConfiguration</typeparam>
		/// </summary>
		/// <param name="selector">Identifies the configuration.</param>
		/// <returns>Configuration value.</returns>
		TConfiguration GetConfiguration<TConfiguration>(IConfigurationSelector<TRawData> selector = null)
			where TConfiguration : IConfiguration;
	}
}