namespace Thinktecture.Configuration
{
	/// <summary>
	/// Information about the provided input.
	/// </summary>
	public interface IConversionInstance
	{
		/// <summary>
		/// Indication whether the value is created and ready to be used or has to be created.
		/// </summary>
		bool IsCreated { get; }

		/// <summary>
		/// Value to be used for population.
		/// </summary>
		object Value { get; }
	}
}
