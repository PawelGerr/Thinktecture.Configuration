using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Thinktecture.Helpers;
using Xunit.Abstractions;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public abstract class ConvertBase
	{
		protected ILogger<MicrosoftConfigurationConverter> Logger;
		protected Mock<IInstanceCreator> InstanceCreatorMock;
		protected MicrosoftConfigurationConverter Converter;

		protected ConvertBase(ITestOutputHelper outputHelper)
		{
			Logger = CreateLogger(outputHelper);
			InstanceCreatorMock = new Mock<IInstanceCreator>(MockBehavior.Strict);
			Converter = new MicrosoftConfigurationConverter(Logger, InstanceCreatorMock.Object);

			InstanceCreatorMock.Setup(c => c.CreateArray(typeof(int), It.IsAny<int>())).Returns<Type, int>((t, l) => new int[l]);
			InstanceCreatorMock.Setup(c => c.Create(typeof(string), It.IsAny<string>())).Returns<Type, string>((t, v) => new ConversionResult(v));
		}

		private ILogger<MicrosoftConfigurationConverter> CreateLogger(ITestOutputHelper outputHelper)
		{
			var serilog = new LoggerConfiguration()
			              .WriteTo.TestOutput(outputHelper)
			              .CreateLogger();

			return new LoggerFactory()
			         .AddSerilog(serilog)
			         .CreateLogger<MicrosoftConfigurationConverter>();
		}

		protected void SetupCreateFromString<T>(string input, T output)
		{
			SetupCreateFromString<T>(input, new ConversionResult(output));
		}

		protected void SetupCreateFromString<T>(string input, IConversionResult result)
		{
			SetupCreateFromString<T>(input, v => result);
		}

		protected void SetupCreateFromString<T>(string input, Func<string, IConversionResult> resultSelector)
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(T), input)).Returns<Type, string>((t, v) => resultSelector(v));
		}

		protected void SetupCreate<T>(T output)
		{
			SetupCreate<T>(new ConversionResult(output));
		}

		protected void SetupCreate<T>(IConversionResult result)
		{
			InstanceCreatorMock.Setup(c => c.Create(typeof(T))).Returns<Type>(t => result);
		}

		protected T RoundtripConvert<T>(T value, bool registerDefaultCreator = true)
			where T : new()
		{
			var config = GetConfig(value);
			return ConvertFrom<T>(config, registerDefaultCreator);
		}

		protected T RoundtripConvert<T>(string key, string value, bool registerDefaultCreator = true)
			where T : new()
		{
			return RoundtripConvert<T>(dictionary => dictionary.Add(key, value), registerDefaultCreator);
		}

		protected T RoundtripConvert<T>(Action<T> populate, bool registerDefaultCreator = true)
			where T : new()
		{
			if (populate == null)
				throw new ArgumentNullException(nameof(populate));

			var instance = new T();
			populate(instance);
			return RoundtripConvert(instance, registerDefaultCreator);
		}

		protected T RoundtripConvert<T>(Action<IDictionary<string, string>> addValues, bool registerDefaultCreator = true)
			where T : new()
		{
			var config = GetConfig(addValues);
			return ConvertFrom<T>(config, registerDefaultCreator);
		}

		private T ConvertFrom<T>(IConfiguration config, bool registerDefaultCreator)
			where T : new()
		{
			if (registerDefaultCreator)
				InstanceCreatorMock.Setup(creator => creator.Create(It.Is<Type>(type => type == typeof(T)))).Returns(() => new ConversionResult(new T()));

			return Converter.Convert<T>(config);
		}

		protected IConfigurationRoot GetConfig(object obj)
		{
			var config = new ConfigurationBuilder()
			             .AddJsonFile(new TestFileProvider(obj), "configuration.json", false, false)
			             .Build();

			return config;
		}

		private static IConfigurationRoot GetConfig(Action<IDictionary<string, string>> addValues)
		{
			var values = new Dictionary<string, string>();
			addValues?.Invoke(values);

			var config = new ConfigurationBuilder()
			             .AddInMemoryCollection(values)
			             .Build();

			return config;
		}
	}
}
