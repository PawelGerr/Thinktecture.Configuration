using System;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Indication an error during (de)serialization of a configuration.
	/// </summary>
	public class ConfigurationSerializationException : Exception
	{
		/// <summary>
		/// Initializes new instance of <see cref="ConfigurationSerializationException"/>.
		/// </summary>
		/// <param name="message">Error message.</param>
		public ConfigurationSerializationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes new instance of <see cref="ConfigurationSerializationException"/>.
		/// </summary>
		/// <param name="message">Error message.</param>
		/// <param name="innerException">Inner exception.</param>
		public ConfigurationSerializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
