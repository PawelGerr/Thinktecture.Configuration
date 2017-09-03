using System;
using JetBrains.Annotations;

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
		[NotNull]
		IConversionResult Create([NotNull] Type type);

		/// <summary>
		/// Creates new instance of provided <paramref name="type"/> using the provided <paramref name="value"/>.
		/// </summary>
		/// <param name="type">Type an instance to create of.</param>
		/// <param name="value">Value to use when creating an instance of type <paramref name="type"/>.</param>
		/// <returns>New instance of type <paramref name="type"/>.</returns>
		[NotNull]
		IConversionResult Create([NotNull] Type type, [CanBeNull] string value);

		/// <summary>
		/// Creates a new array of provided <paramref name="elementType"/> and with provided <paramref name="length"/>.
		/// </summary>
		/// <param name="elementType">The type of the array elements.</param>
		/// <param name="length">The length of the array.</param>
		/// <returns>New instance of <see cref="Array"/>.</returns>
		[NotNull]
		Array CreateArray([NotNull] Type elementType, int length);
	}
}
