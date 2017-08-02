using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Autofac;
using Autofac.Core.Registration;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Configuration.AutofacInstanceCreatorTests
{
	public class Create_Collection
	{
		private readonly AutofacInstanceCreator _creator;

		public Create_Collection()
		{
			var builder = new ContainerBuilder()
				.RegisterDefaultMicrosoftConfigurationTypes();
			_creator = new AutofacInstanceCreator(builder.Build(), CultureInfo.InvariantCulture);
		}

		private void Should_create_list<T>(Type collectionType)
		{
			var collection = _creator.Create(collectionType);
			collection.IsValid.Should().Be(true);
			collection.Value.Should().BeOfType<List<T>>();
		}

		private void Should_throw(Type collectionType)
		{
			Action  action = () => _creator.Create(collectionType);
			action.ShouldThrow<ComponentNotRegisteredException>();
		}

		[Fact]
		public void Should_create_ienumerable_of_int()
		{
			Should_create_list<int>(typeof(IEnumerable<int>));
		}

		[Fact]
		public void Should_throw_if_ienumerable_is_not_generic()
		{
			Should_throw(typeof(IEnumerable));
		}

		[Fact]
		public void Should_create_icollection_of_int()
		{
			Should_create_list<int>(typeof(ICollection<int>));
		}

		[Fact]
		public void Should_throw_if_icollection_is_not_generic()
		{
			Should_throw(typeof(ICollection));
		}

		[Fact]
		public void Should_create_ilist_of_int()
		{
			Should_create_list<int>(typeof(IList<int>));
		}

		[Fact]
		public void Should_thorw_if_ilist_is_not_generic()
		{
			Should_throw(typeof(IList));
		}

		[Fact]
		public void Should_create_ireadonlycollection_of_int()
		{
			Should_create_list<int>(typeof(IReadOnlyCollection<int>));
		}

		[Fact]
		public void Should_create_ireadonlylist_of_int()
		{
			Should_create_list<int>(typeof(IReadOnlyList<int>));
		}

		[Fact]
		public void Should_create_collection_of_int()
		{
			var collection = _creator.Create(typeof(Collection<int>));
			collection.Value.Should().BeOfType<Collection<int>>();
		}

		[Fact]
		public void Should_create_list_of_int()
		{
			Should_create_list<int>(typeof(List<int>));
		}
	}
}