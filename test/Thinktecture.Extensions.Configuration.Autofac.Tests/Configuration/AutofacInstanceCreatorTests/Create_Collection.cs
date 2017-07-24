using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Autofac;
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
			collection.Should().BeOfType<List<T>>();
		}
		
		[Fact]
		public void Should_create_ienumerable_of_int()
		{
			Should_create_list<int>(typeof(IEnumerable<int>));
		}

		[Fact]
		public void Should_create_icollection_of_int()
		{
			Should_create_list<int>(typeof(ICollection<int>));
		}
		
		[Fact]
		public void Should_create_ilist_of_int()
		{
			Should_create_list<int>(typeof(IList<int>));
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
			collection.Should().BeOfType<Collection<int>>();
		}

		[Fact]
		public void Should_create_list_of_int()
		{
			Should_create_list<int>(typeof(List<int>));
		}
	}
}