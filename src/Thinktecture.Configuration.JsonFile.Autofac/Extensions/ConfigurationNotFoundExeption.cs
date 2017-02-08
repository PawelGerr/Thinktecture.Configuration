using System;

namespace Thinktecture.Extensions
{
	/// <summary>
	/// Throw when configuration hasn't been found.
	/// </summary>
	public class ConfigurationNotFoundExeption : Exception
	{
		/// <summary>
		/// Type of the configuration.
		/// </summary>
		public Type Type { get; }

		/// <summary>
		/// The name of the json property
		/// </summary>
		public string PropertyName { get; }

		/// <summary>
		/// Creates new instance of <see cref="ConfigurationNotFoundExeption"/>.
		/// </summary>
		/// <param name="type">Type of the configuration.</param>
		/// <param name="propertyName">Json property name.</param>
		public ConfigurationNotFoundExeption(Type type, string propertyName = null)
			: base($"Configuration not found. Type: {type?.Name}. Property name: {propertyName}")
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			Type = type;
			PropertyName = propertyName;
		}
	}
}