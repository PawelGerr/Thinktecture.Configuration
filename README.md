[![Build status](https://ci.appveyor.com/api/projects/status/j4nbnc9m8yj5g6bw?svg=true)](https://ci.appveyor.com/project/PawelGerr/thinktecture-configuration)
[![Thinktecture.Configuration](https://img.shields.io/nuget/v/Thinktecture.Configuration.svg?maxAge=3600)](https://www.nuget.org/packages/Thinktecture.Configuration/)

Configuration is one of the most prominent cornerstones in software systems, and especially in distributed systems. And it has been a point for discussions in .NET for quite some time.

In one of our projects we have built a solution that lets different applications in different companies exchange data, although being behind firewalls, using the open source [Relay Server](http://thinktecture.com/relayserver). But, to our surprise, one of the features our customer was amazed about was the library I've developed to make configuration easier to handle.

In Thinktecture.Configuration I've taken over the ideas, generalized them and added new features.

The basic idea is that .NET developers should be able to deal with configuration data by just using arbitrary classes.

The library consists of 3 main components: `IConfigurationLoader`, `IConfigurationProvider` and `IConfigurationSelector`. The `IConfigurationLoader` loads data from storage, e.g. JSON from the file system. The `IConfigurationProvider` uses `IConfigurationSelector` to select the correct piece of data like a JSON property and to convert this data to requested configuration.

 ![Architecture](https://github.com/PawelGerr/Thinktecture.Configuration/raw/master/docs/Architecture.png)

In short, the features of the lib are:

* The configuration (i.e. the type being used by the application)
  * should be type-safe
  * can be an abstract type (e.g. an interface)
  * don't have to have property setters or other kind of metadata just to be deserializable
  * can be and have properties that are virtually as complex as needed
  * can be injected via DI (dependency injection)
  * can have dependencies that are injected via DI (i.e. via constructor injection)
  * can be changed by "overrides"
* The usage of the configuration in a developer's code base should be free of any stuff from the configuration library
* The extension of a configuration by new properties or changing the structure of it should be a one-liner

# Use Cases

**Thinktecture.Configuration** is able to use 2 kind of sources:
* [IConfiguration](https://github.com/aspnet/Configuration) which in turn has its own sources like [Environment Variables](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.EnvironmentVariables), [JSON Files](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json), [Command Line](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.CommandLine), [XML](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Xml), etc.
* JSON files only

Although the latter one seems to be unnecessary it has some benefits over `IConfiguration`.
* `IConfiguration` suffers from information loss. For examples
    * If you are using a JSON file as the source and your configuration contains an array-property then you can't differentiate between (1) the array is empty `{ "myArray": [] }` and (2) array is missing at all `{ }`. The same goes for other reference types, i.e. classes.
    * Furthermore, `IConfiguration` is missing type information, it is a collection of key-value pairs of type `string`
    * If one of the keys contain accidentally the character `:` like `my:key` then the configuration `{ "my:key": 42 }` will be interpreted as `{ "my": { "key": 42 } }`
* For deserialization of JSON you can use very powerful library [Newtonsoft JSON](http://www.newtonsoft.com/json). For `IConfiguration` we have the [ConfigurationBinder](https://github.com/aspnet/Configuration/tree/dev/src/Microsoft.Extensions.Configuration.Binder), which is being used by [IOptions\<T>](https://docs.microsoft.com/en-us/aspnet/core/api/microsoft.extensions.options.ioptions-1), but the binder is not extensible because it is a `static` class.

All in all the `IConfiguration` is more versatile but using JSON files only is a more powerful approach.

To get a better feeling of the library read the next section about how to **[Use JSON Files as Data Source](#Use-JSON-Files-as-Data-Source)** or jump right to **[Use `IConfiguration` as Data Source](#use-iconfiguration-as-data-source)**

## Use JSON Files as Data Source

In this post I'm going to show the capabilities of the library by illustrating it with a few examples. In this concrete example I'am using a JSON file containing the configuration values (with [Newtosonf.Json](http://www.newtonsoft.com/json) in the background) and [Autofac](https://autofac.org/) for DI.

But the library is not limited to these. The hints are at the end of this post if you want to use a different DI framework, other storage than the file system (i.e. JSON files) or not use JSON altogether.

The first use case is a bit lengthy to explain the basics. The others will just point out specific features.

Nuget: `Install-Package Thinktecture.Configuration.JsonFile.Autofac`

### 1. One file containing one or more configurations

Shown features in this example:

* one JSON file
* multiple configurations
* a configuration doesn't have to be on the root of the JSON file
* a configuration has dependencies known by DI
* a configuration gets injected in a component like any other dependency

We start with 2 simple configuration types.

```
public interface IMyConfiguration
{
    string Value { get; }
}

public interface IOtherConfiguration
{
    TimeSpan Value { get; }  
}
```

The configuration `IMyConfiguration` is required by our component `MyComponent`.

```
public class MyComponent
{
    public MyComponent(IMyConfiguration config)
    {
    }
}
```

Configuration file `configuration.json`

```
{
    "My":
    {
        "Config": { value: "content" }
    },
    "OtherConfig": { value: "00:00:05" }
}
```

Now let's setup the code in the executing assembly and configure DI to make `MyComponent` resolvable along with `IMyConfiguration`.

```
var builder = new ContainerBuilder();
builder.RegisterType<MyComponent>().AsSelf();

// IFile is required by JsonFileConfigurationLoader to access the file system
// For more info: https://www.nuget.org/packages/Thinktecture.IO.FileSystem.Abstractions/
builder.RegisterType<FileAdapter>().As<IFile>().SingleInstance();

// register so-called configuration provider that operates on "configuration.json"
builder.RegisterJsonFileConfigurationProvider("./configuration.json");

// register the configuration.
// "My.Config" is the (optional) path into the config JSON structure because our example configuration does not start at the root of the JSON
builder.RegisterJsonFileConfiguration<MyConfiguration>("My.Config")
    .AsImplementedInterfaces() // i.e. as IMyConfiguration
    .SingleInstance(); // The values won't change in my example

// same difference with IOtherConfiguration
builder.RegisterJsonFileConfiguration<OtherConfiguration>("OtherConfig")
    .AsImplementedInterfaces();

var container = builder.Build();
```

The concrete types `MyConfiguration` and `OtherConfiguration` are, as often when working with abstractions, used with DI only. Apart from that, these types won't show up at any other places. The type `MyConfiguration` has a dependency `IFile` that gets injected during deserialization.

```
public class MyConfiguration : IMyConfiguration
{
    public string Value { get; set; }

    public MyConfiguration(IFile file)
    {
        ...
    }
}

public class OtherConfiguration : IOtherConfiguration
{
    public TimeSpan Value { get; set; }  
}
```

The usage is nothing special

```
// IMyConfiguration gets injected into MyComponent
var component = container.Resolve<MyComponent>();

// we can resolve IMyConfiguration directly if we want to
var config = container.Resolve<IMyConfiguration>();
```

### 2. Nesting

Shown features in this use case:

* one of the properties of a configuration type is a complex type
* complex property type can be instantiated by Newtonsoft.Json or DI
* complex property can be resolved directly if required

In this example `IMyConfiguration` has a property that is not of a simple type. The concrete types implementing `IMyConfiguration` and `IMyClass` consist of property getters and setters only thus left out for brevity.

```
public interface IMyConfiguration
{
    string Value { get; }
    IMyClass MyClassValue { get; }
}

public interface IMyClass
{
    int Value { get; }  
}
```

The JSON file looks as following:

```
{
    "Value": "content",
    "MyClassValue": { "Value": 42 }
}
```

Having a complex property we can decide whether the type `IMyClass` is going to be instantiated by Newtonsoft.Json or DI.

With just the following line the type `IMyClass` is not introduced to the configuration library and is going to be instantiated by Newtonsoft.Json.

```
builder.RegisterJsonFileConfiguration<MyConfiguration>()
    .AsImplementedInterfaces()
    .SingleInstance();
```

With the following line we introduce the type to the config lib and DI but the instance of `IMyClass` cannot be resolved directly.

```
builder.RegisterJsonFileConfigurationType<MyClass>();
```

Should `IMyClass` be resolvable directly then we can use the instance created along with `IMyConfiguration` or let new instance be created.

```
// option 1: use the property of IMyConfiguration
builder.Register(context => context.Resolve<IMyConfiguration>().MyClassValue)
    .AsImplementedInterfaces()
    .SingleInstance();

// option 2: let create a new instance 
builder.RegisterJsonFileConfiguration<MyClass>("MyClassValue")
    .AsImplementedInterfaces()
    .SingleInstance();
```

### 3. Multiple JSON files

The configurations can be loaded from more than one file.

Configuration types are

```
public interface IMyConfiguration
{
    string Value { get; }
}

public interface IOtherConfiguration
{
    TimeSpan Value { get; }  
}
```

File `myConfiguration.json`

```
{
    "Value": "content"
}
```

File `otherConfiguration.json`

```
{
    "Value": "00:00:05"
}
```

Having two files we need a means to distinguish between them when registering the configurations. In this case we use `RegisterKeyedJsonFileConfigurationProvider` that returns a key that will be passed to `RegisterJsonFileConfiguration`.

```
var providerKey = builder.RegisterKeyedJsonFileConfigurationProvider("myConfiguration.json");
builder.RegisterJsonFileConfiguration<MyConfiguration>(providerKey)
    .AsImplementedInterfaces()
    .SingleInstance();

var otherKey = builder.RegisterKeyedJsonFileConfigurationProvider("otherConfiguration.json");
builder.RegisterJsonFileConfiguration<OtherConfiguration>(otherKey)
    .AsImplementedInterfaces()
    .SingleInstance();
```

### 4. Overrides

A configuration can be assembled from one base configuration and one or more overrides.

In this case we have two config files. One containing the default values of the configuration and the other containing values to override.

Default values come from `baseConfiguration.json`

```
{
    "Value":
    {
        "InnerValue_1": 1,
        "InnerValue_2": 2
    }
}
```

`InnerValue_2` will be changed by the `overrides.json`

```
{
    "Value":
    {
        "InnerValue_2": 3
    }
}
```

The configuration 

```
public interface IMyConfiguration
{
    IInnerConfiguration Value { get; }  
}

public interface IInnerConfiguration
{
    int InnerValue_1 { get; }
    int InnerValue_2 { get; }
}
```

To specify overrides we need to provide more than one file path when registering the configuration provider. The overrides are applied in the same order they passed to `RegisterJsonFileConfigurationProvider`.

```
builder.RegisterJsonFileConfigurationProvider("baseConfiguration.json", "overrides.json");
builder.RegisterJsonFileConfiguration<MyConfiguration>()
    .AsImplementedInterfaces()
    .SingleInstance();
```

### 5. Extension of the configuration

Let's add a property to `IInnerConfiguration` from previous paragraph.

```
public interface IInnerConfiguration
{
    int InnerValue_1 { get; }
    int InnerValue_2 { get; }
    string NewValue { get; }
}
```

Add the corresponding property to the JSON file `baseConfiguration.json`

```
{
    "Value":
    {
        "InnerValue_1": 1,
        "InnerValue_2": 2,
        "NewValue": "content"
    }
}
```

That's it.

## Use [IConfiguration](https://github.com/aspnet/Configuration) as Data Source

The `IConfiguration` itself is more or less just a collection of key-value pairs of type `string`. Using it as-it-is in a library is not recommended because (1) the depevelopers on both side (lib owner and lib consumer) have to know the *keys* of the configuration values and (2) how to (de)serialize the values.

`Thinktecture.Extensions.Configuration` is using an instance of `IConfiguration` as a data source to create and populate strongly-typed configuration classes. 

The Library [Microsoft.Extensions.Options](https://www.nuget.org/packages/Microsoft.Extensions.Options/) provides similar solution but it is using a static class from [Microsoft.Extensions.Configuration.Binder](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Binder/) making it hard to extend. `Thinktecture.Extensions.Configuration` provides new features like
* DI support (e.g. for Autofac use nuget package `Thinktecture.Extensions.Configuration.Autofac`)
* `CultureInfo` support, so that localized values like `42,1` with `,` as decimal separator are parsed correctly to `42.1`
* Honoring of collection indexes, meaning a key-value pair `"MyIntCollection:1"`-`42` is deserialized to `new int[]{0, 42}` instead of `new int[]{42}`
* No polution of own code by *helper classes/interfaces* (`IOptions<T>`, `IOptionsSnapshot<T>`, etc) by default
  * Still with reload support
* If a property of type `int[]` (ie. an array) is comming from a JSON file and is set to `null`, ie. JSON is looking like this `{ "myIntArray": null }` then `IOptions<T>` throws an error because the provider [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json) generates an empty string if it sees `null`. This incompatibility between the provider [Microsoft.Extensions.Configuration.Json](https://www.nuget.org/packages/Microsoft.Extensions.Configuration.Json) and `IOptions<T>` is very unfortunate. `Thinktecture.Extensions.Configuration` sets the property to `null` in this case.

### Registering IConfiguration as data source for `Thinktecture.Configuration`

Nuget: `Install-Package Thinktecture.Extensions.Configuration.Autofac` when using [Autofac](https://autofac.org/)

Use the extension method `RegisterMicrosoftConfigurationProvider` or `RegisterKeyedMicrosoftConfigurationProvider` when having more than 1 instance of `IConfiguration`. The latter one is more of an edge case.

```
IConfiguration config = ...;
ContainerBuilder builder = ...; // Autofac container builder

builder.RegisterMicrosoftConfigurationProvider(config);
```

### Registering of strongly-typed Configurations

Use the extension method `RegisterMicrosoftConfiguration` to register a class with the DI.

```
builder.RegisterMicrosoftConfiguration<MyConfiguration>()
    .As<IMyConfiguration>()     // from here on we have standard Autofac registration builder
    .InstancePerLifetimeScope();
```

If the type `IMyConfiguration` contains a property of a complex type, say `IMyOtherConfiguration` ...

```
public interface IMyConfiguration
{
	int MyValue { get; }
	IMyOtherConfiguration InnerConfiguration { get; }
}
```

... then it has to be registered with DI as well:

```
builder.RegisterMicrosoftConfigurationType<MyOtherConfiguration, IMyOtherConfiguration>();
```

The configuration of type `IMyConfiguration` can now be resolved

```
var container = builder.Build();
var myConfig = container.Resolve<IMyConfiguration>();
```

# Working with different frameworks, storages and data models

## Using another DI framework

To use a different DI framwork than Autofac use the package `Thinktecture.Configuration.JsonFile` instead of `Thinktecture.Configuration.JsonFile.Autofac` and implement the interface `IJsonTokenConverter` using your favorite DI framework.  The converter has just one method `TConfiguration Convert<TConfiguration>(JToken token)`.

## Load JSON from other media

To load `JToken` from other storages than the file system just implement the interface `IConfigurationLoader<JToken, JToken>`. For example, if the JSON configuration are in a database then inject the database context or a data access layer and select corresponding data rows.

## Use different data models

If you are using other data model than JSON then reference the package Thinktecture.Configuration and implement the interfaces `IConfigurationLoader<TRawDataIn,TRawDataOut>`, `IConfigurationProvider<TRawDataIn,TRawDataOut>` and `IConfigurationSelector<TRawDataIn,TRawDataOut>`. It sounds like much but if you look into the code of the corresponding JSON-based classes you will see that the classes are pretty small and trivial.

# Some final words...

Although the configuration is an important part of the software development it is not the most exciting one. Therefore, a software developer may be inclined to take shortcuts and work with meaningful hardcoded values. `Thinktecture.Configuration` gives you the means to work with .NET types without thinking too much how to load and parse the values. This saves time, improves the reusability of the components and the software architecture.
