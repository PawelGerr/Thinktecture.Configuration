using System;
using System.Linq;
using JetBrains.Annotations;
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
		public JsonFileConfigurationSelector([NotNull] string propertyPath)
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
		[CanBeNull]
		public JToken Select([NotNull] JToken token)
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

		private JToken GetChild([NotNull] JToken token, [NotNull] string propertyName)
		{
			if (token == null)
				throw new ArgumentNullException(nameof(token));
			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));

			foreach (var child in token)
			{
				if ((child is JProperty prop) && StringComparer.OrdinalIgnoreCase.Equals(prop.Name, propertyName))
					return prop.Value;
			}
			return null;
		}
	}
}
