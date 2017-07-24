using System;
using System.Collections.Generic;
using System.Globalization;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Configuration.AutofacInstanceCreatorTests
{
	public class Create_Dictionary
	{
		private readonly AutofacInstanceCreator _creator;

		public Create_Dictionary()
		{
			var builder = new ContainerBuilder()
				.RegisterDefaultMicrosoftConfigurationTypes();
			_creator = new AutofacInstanceCreator(builder.Build(), CultureInfo.InvariantCulture);
		}

		private void Should_create_dictionary<TKey, TValue>(Type collectionType)
		{
			var collection = _creator.Create(collectionType);
			collection.Should().BeOfType<Dictionary<TKey, TValue>>();
		}

		[Fact]
		public void Should_create_idictionary_of_string_int()
		{
			Should_create_dictionary<string, int>(typeof(IDictionary<string, int>));
		}

		[Fact]
		public void Should_create_dictionary_of_string_int()
		{
			Should_create_dictionary<string, int>(typeof(Dictionary<string, int>));
		}

		[Fact]
		public void Should_create_ireadonlydictionary_of_string_int()
		{
			Should_create_dictionary<string, int>(typeof(IReadOnlyDictionary<string, int>));
		}
	}
}