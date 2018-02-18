namespace Thinktecture.Configuration
{
	/// <summary>
	/// The result of the conversion.
	/// </summary>
	public interface IConversionResult
	{
		/// <summary>
		/// Indicates whether the conversion is successful and the value should be used to populate the configuration,
		/// otherwise the value will be skipped.
		/// </summary>
		bool IsValid { get; }

		/// <summary>
		/// The value after conversion.
		/// </summary>
		object Value { get; }
	}
}
