using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Autofac;
using FluentAssertions;
using Xunit;

namespace Thinktecture.Configuration.AutofacInstanceCreatorTests
{
	public class Create_FromValue
	{
		private AutofacInstanceCreator _creator;

		public Create_FromValue()
		{
			Initialize(CultureInfo.InvariantCulture);
		}

		private void Initialize(CultureInfo cultureInfo)
		{
			var builder = new ContainerBuilder()
				.RegisterDefaultMicrosoftConfigurationTypes();
			_creator = new AutofacInstanceCreator(builder.Build(), cultureInfo);
		}

		[Fact]
		public void Should_create_string()
		{
			_creator.Create(typeof(string), "foo")
				.Should().Be("foo");
		}

		[Fact]
		public void Should_create_int()
		{
			_creator.Create(typeof(int), "42")
				.Should().Be(42);
		}

		[Fact]
		public void Should_create_decimal()
		{
			_creator.Create(typeof(decimal), "42.1")
				.Should().Be(42.1m);
		}

		[Fact]
		public void Should_create_decimal_with_de_de()
		{
			Initialize(new CultureInfo("de-DE"));

			_creator.Create(typeof(decimal), "42,1")
				.Should().Be(42.1m);
		}

		[Fact]
		public void Should_create_double()
		{
			_creator.Create(typeof(double), "42.1")
				.Should().Be(42.1d);
		}

		[Fact]
		public void Should_create_double_with_de_de()
		{
			Initialize(new CultureInfo("de-DE"));

			_creator.Create(typeof(double), "42,1")
				.Should().Be(42.1d);
		}

		[Fact]
		public void Should_create_boolean()
		{
			_creator.Create(typeof(bool), "True")
				.Should().Be(true);
		}
	}
}