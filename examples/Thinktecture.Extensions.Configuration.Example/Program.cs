using System;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Thinktecture.Extensions.Configuration.Example.Configuration;

namespace Thinktecture.Extensions.Configuration.Example
{
	public static class Program
	{
		// ReSharper disable once FunctionNeverReturns
		public static void Main()
		{
			var loggerConfig = new LoggerFactory()
				.AddConsole(LogLevel.Trace);

			IConfiguration config = new ConfigurationBuilder()
			                        .AddJsonFile("configuration.json", false, true)
			                        .Build();

			var builder = new ContainerBuilder();
			builder.RegisterInstance(loggerConfig).As<ILoggerFactory>();
			builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>)).SingleInstance();
			builder.RegisterType<MyDependency>().AsSelf();

			builder.RegisterMicrosoftConfigurationProvider(config);
			builder.RegisterMicrosoftConfiguration<MyConfiguration>().As<IMyConfiguration>();
			builder.RegisterMicrosoftConfigurationType<MyInnerConfiguration, IMyInnerConfiguration>();

			//var key = builder.RegisterKeyedMicrosoftConfigurationProvider(config);
			//builder.RegisterMicrosoftConfiguration<MyConfiguration>(key).As<IMyConfiguration>();

			var container = builder.Build();
			var myConfig = container.Resolve<IMyConfiguration>();

			Console.WriteLine($"myConfig.ClassWithTypeConverter.Prop: {myConfig.ClassWithTypeConverter?.Prop}");
			Console.WriteLine($"StructConfiguration.Value: {myConfig.StructConfiguration.Value}");

			while (true)
			{
				var myConfig2 = container.Resolve<IMyConfiguration>();

				Console.WriteLine($"myConfig.Value: {myConfig.Value}");
				Console.WriteLine($"myConfig2.Value: {myConfig2.Value}");
				Console.WriteLine($"myConfig == myConfig2: {ReferenceEquals(myConfig, myConfig2)}");

				Console.WriteLine("Press ENTER to re-resolve");
				Console.ReadLine();
			}
		}
	}
}
