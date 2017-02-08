using System;
using Autofac;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Creates a type using autofac.
	/// </summary>
	public class AutofacCreationJsonConverter : CustomCreationConverter
	{
		private readonly IComponentContext _container;

		/// <summary>
		/// Creates instances of provided <paramref name="type"/> using autofac.
		/// </summary>
		/// <param name="type">The type the instances are created of.</param>
		/// <param name="container">Autofac container.</param>
		public AutofacCreationJsonConverter(Type type, IComponentContext container)
			: base(type)
		{
			if (container == null)
				throw new ArgumentNullException(nameof(container));

			_container = container;
		}

		/// <inheritdoc />
		public override object Create(Type objectType)
		{
			if (objectType == null)
				throw new ArgumentNullException(nameof(objectType));

			return _container.Resolve(objectType);
		}
	}
}