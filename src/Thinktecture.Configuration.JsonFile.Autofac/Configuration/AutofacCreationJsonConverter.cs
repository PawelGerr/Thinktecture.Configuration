using System;
using Autofac;
using JetBrains.Annotations;

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
		public AutofacCreationJsonConverter([NotNull] Type type, [NotNull] IComponentContext container)
			: base(type)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));
		}

		/// <inheritdoc />
		public override object Create([NotNull] Type objectType)
		{
			if (objectType == null)
				throw new ArgumentNullException(nameof(objectType));

			return _container.ResolveConfigurationType(objectType);
		}
	}
}
