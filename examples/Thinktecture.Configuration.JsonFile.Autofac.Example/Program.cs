using System;
using Autofac;
using Thinktecture.Configuration.JsonFile.Autofac.Example.Configuration;
using Thinktecture.IO;
using Thinktecture.IO.Adapters;

namespace Thinktecture.Configuration.JsonFile.Autofac.Example
{
	public class Program
	{
		private const string _CONFIG_FILE_PATH = "Configuration.json";
		private const string _OTHER_CONFIG_FILE_PATH = "AnotherConfiguration.json";

		public static void Main(string[] args)
		{
			var container = CreateContainer();

			using (var scope = container.BeginLifetimeScope())
			{
				var app = scope.Resolve<IMyApplication>();
				app.PrintConfiguration();

				var otherConfig = scope.Resolve<IMyComponentOtherConfiguration>();
				Console.WriteLine($"{Environment.NewLine}Direct resolution of \"IMyComponentOtherConfiguration\": {otherConfig.OtherComponentValue}");

				var componentConfig = scope.Resolve<IMyComponentConfiguration>();
				Console.WriteLine($"{Environment.NewLine}Is \"IMyComponentConfiguration.OtherConfiguration\" == \"IMyComponentOtherConfiguration\": {(componentConfig.OtherConfiguration == otherConfig ? "yes" : "no")}");

				var configFromOtherFile = scope.Resolve<IConfigurationFromOtherFile>();
				Console.WriteLine($"{Environment.NewLine}\"IConfigurationFromOtherFile\": {configFromOtherFile.Value}");

				Console.WriteLine(Environment.NewLine + "Press ENTER to exit.");
				Console.ReadLine();
			}
		}

		private static IContainer CreateContainer()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<MyApplication>().AsImplementedInterfaces();
			builder.RegisterType<MyComponent>().AsImplementedInterfaces();

			// IFile is required by JsonFileConfigurationLoader to access the file system
			builder.RegisterType<FileAdapter>().As<IFile>().SingleInstance();

			builder.RegisterJsonFileConfigurationProvider(_CONFIG_FILE_PATH);
			builder.RegisterJsonFileConfiguration<MyApplicationConfiguration>().AsImplementedInterfaces().SingleInstance();
			builder.RegisterJsonFileConfiguration<MyComponentConfiguration>("MyComponent").AsImplementedInterfaces().SingleInstance();
			builder.RegisterJsonFileConfigurationType<MyComponentOtherConfiguration>();

			// In case we want to use/resolve IMyComponentOtherConfiguration directly without creating a new instance.
			builder.Register(context => context.Resolve<IMyComponentConfiguration>().OtherConfiguration).AsImplementedInterfaces().SingleInstance();
			// In case we want to use/resolve IMyComponentOtherConfiguration directly and want to create a new instance instead of reusing it from "IMyComponentConfiguration.OtherConfiguration"
			//builder.RegisterJsonFileConfiguration<MyComponentOtherConfiguration>("MyComponent.OtherConfiguration").AsImplementedInterfaces().SingleInstance();

			// We register an new config provider with a key to distinguish between them
			var providerKey = builder.RegisterKeyedJsonFileConfigurationProvider(_OTHER_CONFIG_FILE_PATH);
			builder.RegisterJsonFileConfiguration<ConfigurationFromOtherFile>(providerKey, "ConfiguationFromOtherFile").AsImplementedInterfaces().SingleInstance();
			
			return builder.Build();
		}
	}
}