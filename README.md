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

In this post I'm going to show the capabilities of the library by illustrating it with a few examples. In this concrete example I'am using a JSON file containing the configuration values (with [Newtosonf.Json](http://www.newtonsoft.com/json) in the background) and [Autofac](https://autofac.org/) for DI.

But the library is not limited to these. The hints are at the end of this post if you want to use a different DI framework, other storage than the file system (i.e. JSON files) or not use JSON altogether.

The first use case is a bit lengthy to explain the basics. The others will just point out specific features.

Nuget: `Install-Package Thinktecture.Configuration.JsonFile.Autofac`

## 1. One file containing one or more configurations

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

## 2. Nesting

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

## 3. Multiple JSON files

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

## 4. Overrides

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

## 5. Extension of the configuration

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

# Working with different frameworks, storages and data models

## Using another DI framework

To use a different DI framwork than Autofac use the package `Thinktecture.Configuration.JsonFile` instead of `Thinktecture.Configuration.JsonFile.Autofac` and implement the interface `IJsonTokenConverter` using your favorite DI framework.  The converter has just one method `TConfiguration Convert<TConfiguration>(JToken token)`.

## Load JSON from other media

To load `JToken` from other storages than the file system just implement the interface `IConfigurationLoader<JToken, JToken>`. For example, if the JSON configuration are in a database then inject the database context or a data access layer and select corresponding data rows.

Use different data models

If you are using other data model than JSON then reference the package Thinktecture.Configuration and implement the interfaces `IConfigurationLoader<TRawDataIn,TRawDataOut>`, `IConfigurationProvider<TRawDataIn,TRawDataOut>` and `IConfigurationSelector<TRawDataIn,TRawDataOut>`. It sounds like much but if you look into the code of the corresponding JSON-based classes you will see that the classes are pretty small and trivial.

# Some final words...

Although the configuration is an important part of the software development it is not the most exciting one. Therefore, a software developer may be inclined to take shortcuts and work with meaningful hardcoded values. `Thinktecture.Configuration` gives you the means to work with .NET types without thinking too much how to load and parse the values. This saves time, improves the reusability of the components and the software architecture.