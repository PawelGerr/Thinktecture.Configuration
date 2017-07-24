using System;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Creates new instances of provided type.
	/// </summary>
	public interface IInstanceCreator
	{
		/// <summary>
		/// Creates new instance of provided <paramref name="type"/>.
		/// </summary>
		/// <param name="type">Type an instance to create of.</param>
		/// <returns>New instance of type <paramref name="type"/>.</returns>
		object Create(Type type);

		/// <summary>
		/// Creates new instance of provided <paramref name="type"/> using the provided <paramref name="value"/>.
		/// </summary>
		/// <param name="type">Type an instance to create of.</param>
		/// <param name="value">Value to use when creating an instance of type <paramref name="type"/>.</param>
		/// <returns>New instance of type <paramref name="type"/>.</returns>
		object Create(Type type, string value);

		/// <summary>
		/// Creates a new array of provided <paramref name="elementType"/> and with provided <paramref name="length"/>.
		/// </summary>
		/// <param name="elementType">The type of the array elements.</param>
		/// <param name="length">The length of the array.</param>
		/// <returns>New instance of <see cref="Array"/>.</returns>
		Array CreateArray(Type elementType, int length);
	}
}