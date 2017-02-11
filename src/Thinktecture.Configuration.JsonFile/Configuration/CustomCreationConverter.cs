using System;
using System.Reflection;
using Newtonsoft.Json;

namespace Thinktecture.Configuration
{
	/// <summary>Create a custom object</summary>
	public abstract class CustomCreationConverter : JsonConverter
	{
		private readonly Type _type;

		/// <summary>
		/// Creates new instance of <see cref="CustomCreationConverter"/>.
		/// </summary>
		/// <param name="type">The type to convert.</param>
		/// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="type"/> is <c>null</c>.</exception>
		protected CustomCreationConverter(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			_type = type;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="JsonConverter" /> can write JSON.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
		/// </value>
		public override bool CanWrite => false;

		/// <summary>Writes the JSON representation of the object.</summary>
		/// <param name="writer">The <see cref="JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotSupportedException("CustomCreationConverter should be used for deserialization only.");
		}

		/// <summary>Reads the JSON representation of the object.</summary>
		/// <param name="reader">The <see cref="JsonReader" /> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader == null)
				throw new ArgumentNullException(nameof(reader));
			if (serializer == null)
				throw new ArgumentNullException(nameof(serializer));

			if (reader.TokenType == JsonToken.Null)
				return null;

			var obj = Create(objectType);

			if (obj == null)
				throw new JsonSerializationException("No object created.");

			serializer.Populate(reader, obj);
			return obj;
		}

		/// <summary>
		/// Creates an object which will then be populated by the serializer.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>The created object.</returns>
		public abstract object Create(Type objectType);

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			if (objectType == null)
				throw new ArgumentNullException(nameof(objectType));

#if NETSTANDARD1_3
			return _type.GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
#else
			return _type.IsAssignableFrom(objectType);
#endif
		}
	}
}