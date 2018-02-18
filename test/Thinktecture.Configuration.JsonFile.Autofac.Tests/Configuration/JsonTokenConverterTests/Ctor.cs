using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Autofac;
using FluentAssertions;
using Moq;
using Thinktecture.TestTypes;
using Xunit;

namespace Thinktecture.Configuration.JsonTokenConverterTests
{
	[SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
	public class Ctor
	{
		private readonly Mock<ILifetimeScope> _scopeMock;

		public Ctor()
		{
			_scopeMock = new Mock<ILifetimeScope>(MockBehavior.Strict);
		}

		[Fact]
		public void Should_throw_argnull_if_scope_is_null()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Action ctor = () => new AutofacJsonTokenConverter(null, Enumerable.Empty<AutofacJsonTokenConverterType>());

			ctor.Invoking(a => a()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_throw_argnull_if_enumerable_is_null()
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			Action ctor = () => new AutofacJsonTokenConverter(_scopeMock.Object, null);

			ctor.Invoking(a => a()).ShouldThrow<ArgumentNullException>();
		}

		[Fact]
		public void Should_does_not_call_any_members_on_scope_if_types_enumerable_is_empty()
		{
			new AutofacJsonTokenConverter(_scopeMock.Object, Enumerable.Empty<AutofacJsonTokenConverterType>());
		}

		[Fact]
		public void Should_does_not_call_any_members_on_scope_if_types_enumerable_is_not_empty()
		{
			new AutofacJsonTokenConverter(_scopeMock.Object, new List<AutofacJsonTokenConverterType>() { new AutofacJsonTokenConverterType(typeof(ConfigurationWithDefaultCtor)) });
		}
	}
}
