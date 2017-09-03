using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Selects the value of provided property name.
	/// </summary>
	public class JsonFileConfigurationSelector : IConfigurationSelector<JToken, JToken>
	{
		private readonly string[] _pathFragments;

		/// <summary>
		/// Creates new <see cref="JsonFileConfigurationSelector"/>.
		/// </summary>
		/// <param name="propertyPath">The path of the property to select, e.g. <c>MyProperty.AnotherProperty</c> </param>
		public JsonFileConfigurationSelector(string propertyPath)
		{
			if (propertyPath == null)
				throw new ArgumentNullException(nameof(propertyPath));

			if (String.IsNullOrWhiteSpace(propertyPath))
				throw new ArgumentException("Property path must not be empty.", nameof(propertyPath));

			_pathFragments = propertyPath.Trim().Split(new[] { '.' }, StringSplitOptions.None);

			if (_pathFragments.Contains(String.Empty))
				throw new ArgumentException("Property path must not contains multiple dots next to each other.");
		}

		/// <inheritdoc />
		public JToken Select(JToken token)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));

			foreach (var propertyName in _pathFragments)
			{
				token = GetChild(token, propertyName);
				if (token == null)
					return null;
			}

			return token;
		}

		private JToken GetChild(JToken token, string propertyName)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));
			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));

			foreach (var child in token)
			{
				var prop = child as JProperty;

				if ((prop != null) && StringComparer.OrdinalIgnoreCase.Equals(prop.Name, propertyName))
				{
					return prop.Value;
				}
			}
			return null;
		}
	}
}
