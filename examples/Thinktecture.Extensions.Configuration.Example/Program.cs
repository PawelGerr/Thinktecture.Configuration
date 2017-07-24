using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Thinktecture.Extensions.Configuration.Example.Configuration;

namespace Thinktecture.Extensions.Configuration.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var loggerConfig = new LoggerFactory()
				.AddConsole(LogLevel.Trace);

			var config = new ConfigurationBuilder()
				.AddJsonFile("configuration.json")
				.Build();
			
			var builder = new ContainerBuilder();
			builder.RegisterInstance(loggerConfig).As<ILoggerFactory>();
			builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
			builder.RegisterType<MyDependency>().AsSelf();

			builder.RegisterMicrosoftConfigurationProvider(config);
			builder.RegisterMicrosoftConfiguration<MyConfiguration>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterMicrosoftConfigurationType<MyInnerConfiguration>();

			var container = builder.Build();
			var myConfig = container.Resolve<IMyConfiguration>();

			Console.WriteLine("Finished");
		}
	}
}