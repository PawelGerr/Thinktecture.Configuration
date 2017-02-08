using System;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Selects the value of provided property name.
	/// </summary>
	public class JsonFileConfigurationSelector : IConfigurationSelector<JToken>
	{
		private readonly string _propertyName;

		/// <summary>
		/// Creates new <see cref="JsonFileConfigurationSelector"/>.
		/// </summary>
		/// <param name="propertyName">The name of the property to select.</param>
		public JsonFileConfigurationSelector(string propertyName)
		{
			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));

			_propertyName = propertyName;
		}

		/// <inheritdoc />
		public JToken Select(JToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			foreach (var child in token)
			{
				var prop = child as JProperty;

				if ((prop != null) && StringComparer.OrdinalIgnoreCase.Equals(prop.Name, _propertyName))
					return prop.Value;
			}

			return token[_propertyName];
		}
	}
}