using System.Globalization;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Configuration.AutofacInstanceCreatorTests
{
	public class CreateArray
	{
		private readonly AutofacInstanceCreator _creator;

		public CreateArray()
		{
			var builder = new ContainerBuilder();
			_creator = new AutofacInstanceCreator(builder.Build(), CultureInfo.InvariantCulture);
		}

		[Fact]
		public void Should_create_array_have_correct_type()
		{
			var array = _creator.CreateArray(typeof(int), 0);
			array.Should().BeOfType<int[]>();
		}

		[Fact]
		public void Should_create_array_of_length_0()
		{
			var array = _creator.CreateArray(typeof(int), 0);
			array.Length.Should().Be(0);
		}

		[Fact]
		public void Should_create_array_of_length_2()
		{
			var array = _creator.CreateArray(typeof(int), 2);
			array.Length.Should().Be(2);
		}
	}
}