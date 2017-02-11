using System;
using Autofac;
using FluentAssertions;
using Moq;
using Thinktecture.TestTypes;
using Xunit;

namespace Thinktecture.Configuration.AutofacCreationJsonConverterTests
{
	public class Ctor
	{
		private readonly Mock<IComponentContext> _containerMock;

		public Ctor()
		{
			_containerMock = new Mock<IComponentContext>(MockBehavior.Strict);
		}

		[Fact]
		public void Should_throw_if_type_is_null()
		{
			Action ctor = () => new AutofacCreationJsonConverter(null, _containerMock.Object);

			ctor.Invoking(c => c()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_if_container_is_null()
		{
			Action ctor = () => new AutofacCreationJsonConverter(typeof(ConfigurationWithDefaultCtor), null);

			ctor.Invoking(c => c()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_initialize_new_instance_without_calling_any_members()
		{
			new AutofacCreationJsonConverter(typeof(ConfigurationWithDefaultCtor), _containerMock.Object);
		}
	}
}