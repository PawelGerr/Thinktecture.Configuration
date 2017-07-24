using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Thinktecture.Helpers;

namespace Thinktecture.Configuration.MicrosoftConfigurationConverterTests
{
	public abstract class ConvertBase
	{
		protected Mock<ILogger<MicrosoftConfigurationConverter>> LoggerMock;
		protected Mock<IInstanceCreator> InstanceCreatorMock;
		protected MicrosoftConfigurationConverter Converter;

		protected ConvertBase()
		{
			LoggerMock = new Mock<ILogger<MicrosoftConfigurationConverter>>(MockBehavior.Loose);
			InstanceCreatorMock = new Mock<IInstanceCreator>(MockBehavior.Strict);
			Converter = new MicrosoftConfigurationConverter(LoggerMock.Object, InstanceCreatorMock.Object);
		}

		protected object RoundtripConvert<T>(T value)
			where T : new()
		{
			var config = GetConfig(value);
			return ConvertFrom<T>(config);
		}

		protected T RoundtripConvert<T>(string key, string value)
			where T : new()
		{
			return RoundtripConvert<T>(dictionary => dictionary.Add(key, value));
		}

		protected T RoundtripConvert<T>(Action<IDictionary<string, string>> addValues)
			where T : new()
		{
			var config = GetConfig(addValues);
			return ConvertFrom<T>(config);
		}

		private T ConvertFrom<T>(IConfigurationRoot config)
			where T : new()
		{
			InstanceCreatorMock.Setup(creator => creator.Create(It.Is<Type>(type => type == typeof(T)))).Returns(() => new T());

			return (T) Converter.Convert(config, typeof(T));
		}

		private IConfigurationRoot GetConfig(object obj)
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile(new TestFileProvider(obj), "configuration.json", false, false)
				.Build();

			return config;
		}

		private IConfigurationRoot GetConfig(Action<IDictionary<string, string>> addValues)
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