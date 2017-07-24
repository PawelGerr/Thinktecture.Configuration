using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Autofac;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Creates new instances using autofac.
	/// </summary>
	public class AutofacInstanceCreator : IInstanceCreator
	{
		private readonly IComponentContext _container;
		private readonly CultureInfo _culture;

		/// <summary>
		/// Initializes new instance of type <see cref="AutofacInstanceCreator"/>.
		/// </summary>
		/// <param name="container">Autofac container.</param>
		/// <param name="culture">Culture to be used during conversion.</param>
		public AutofacInstanceCreator(IComponentContext container, CultureInfo culture)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));
			_culture = culture ?? throw new ArgumentNullException(nameof(culture));
		}

		/// <inheritdoc />
		public object Create(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return _container.ResolveConfigurationType(type);
		}

		/// <inheritdoc />
		public object Create(Type type, string value)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			return TypeDescriptor.GetConverter(type).ConvertFromString(null, _culture, value);
		}

		/// <inheritdoc />
		public Array CreateArray(Type elementType, int length)
		{
			if (elementType == null)
				throw new ArgumentNullException(nameof(elementType));

			return Array.CreateInstance(elementType, length);
		}
	}
}