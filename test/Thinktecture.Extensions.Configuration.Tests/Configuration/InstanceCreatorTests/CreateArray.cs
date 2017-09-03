using System.Globalization;
using FluentAssertions;
using Thinktecture.Helpers;
using Xunit;

namespace Thinktecture.Configuration.InstanceCreatorTests
{
	public class CreateArray
	{
		private readonly TestInstanceCreator _creator;

		public CreateArray()
		{
			_creator = new TestInstanceCreator(CultureInfo.InvariantCulture);
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
