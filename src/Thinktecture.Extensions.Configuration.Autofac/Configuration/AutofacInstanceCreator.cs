using System;
using System.Globalization;
using Autofac;
using JetBrains.Annotations;

namespace Thinktecture.Configuration
{
	/// <summary>
	/// Creates new instances using autofac.
	/// </summary>
	public class AutofacInstanceCreator : InstanceCreator
	{
		private readonly IComponentContext _container;

		/// <summary>
		/// Initializes new instance of type <see cref="AutofacInstanceCreator"/>.
		/// </summary>
		/// <param name="container">Autofac container.</param>
		/// <param name="culture">Culture to be used during conversion.</param>
		public AutofacInstanceCreator([NotNull] IComponentContext container, [NotNull] CultureInfo culture)
			: base(culture)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));
		}

		/// <inheritdoc />
		public override IConversionResult Create(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			var instance = _container.ResolveConfigurationType(type);

			return new ConversionResult(instance);
		}
	}
}
