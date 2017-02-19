In one of our projects we have built a solution that lets different applications in different companies to exchange data although being behind firewalls using the [Relay Server](http://thinktecture.com/relayserver). But, to our suprise, the feature our customer was amazed the most was the configuration library I've developed.

In `<<Thinktecture.Configuration>>` I've taken over the ideas, generalized them and added new features.

In short, the features are:

* The configuration (i.e. the type being used by the application)
should be type-safe
  * can be an abstract type (e.g. an interface)
  * don't have to have property setters or other kind of metadata just to be deserializable
  * can be and have properties that are virtually as complex as needed
  * can be injected via DI (dependency injection)
  * can have dependencies that are injetected via DI (i.e. via constructor injection)
  * can be changed by "overrides"
* The usage of the configuration should be free of any stuff from the configuration library
* The extension of a configuration by new properties or changing the structure of it should be a *one-liner*

# Use Cases

In this post I'm going to show the capabilities of the library by a few examples. In this concrete example I'am using a json file containing the configuration values (with [Newtosonf.Json](http://www.newtonsoft.com/json) in the background) and [Autofac](https://autofac.org/) but the library is not limited to these. The hints are at the end of this post if you want to use a different DI framework, other storage than the file system (i.e. json files) or json altogether.

The first use case is a bit lengly to explain the basics the others will just point out specific features.

Want to see the code? Go to [link to github](link)

## 1. One file containing one or more configurations

Shown features in this example:

* one json file
* multiple configurations
* a configuration don't have to be on the root of the json file
* a configuration has dependecies known by DI
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

Now let's setup DI to make `MyComponent` along with the `IMyConfiguration` resolvable.

```
var builder = new ContainerBuilder();
builder.RegisterType<MyComponent>().AsSelf();

// IFile is required by JsonFileConfigurationLoader to access the file system
// For more info: https://www.nuget.org/packages/Thinktecture.IO.FileSystem.Abstractions/
builder.RegisterType<FileAdapter>().As<IFile>().SingleInstance();

// register so called configuration provider that operates on "configuration.json"
builder.RegisterJsonFileConfigurationProvider("./configuration.json");

// register the configuration.
// "My.Config" is the (optional) json path because our example configuration does not start at the root of the json
builder.RegisterJsonFileConfiguration<MyConfiguration>("My.Config")
    .AsImplementedInterfaces() // i.e. as IMyConfiguration
    .SingleInstance(); // The values won't change in my example

// same difference with IOtherConfiguration
builder.RegisterJsonFileConfiguration<OtherConfiguration>("OtherConfig")
    .AsImplementedInterfaces();

var container = builder.Build();
```

The concrete types `MyConfiguration` and `OtherConfiguration` are, as often when working with abstractions, used with DI only. Apart from that, this types won't show up at any other places. The type `MyConfiguration` has a dependency `IFile` that gets injected during deserialization.

```
public class MyConfiguration : IMyConfiguration
{
    public string Value { get; set; }

    public MyConfiguration(IFile file)
    {
        ...
    }
}

// same difference with OtherConfiguration
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

* one of the properties of a configuration is a complex type
* complex property type can be instantiated by Newtonsoft.Json or DI
* complex property can be resolved directly if required

In this example `IMyConfiguration` has a property that is not of a trivial type. The implementations of both interfaces `IMyConfiguration` and `IMyClass` are just getter and setter and let out for brevity.

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

The json file looks as following:

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

## 3. Multiple json files

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

Having 2 files we needs a means to distinguish between them when registering the configurations. In this case we use `RegisterKeyedJsonFileConfigurationProvider` that returns a key that will be passed to `RegisterJsonFileConfiguration`.

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

In the case we have 2 file. One containing the default values of the configuration and the other containing values to override.

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

# Working with differnt frameworks, storages and data models

## Using another DI framework

To use a different DI framwork than Autofac use the package `<<Thinktecture.Configuration.JsonFile>>` instead of `<<Thinktecture.Configuration.JsonFile.Autofac>>` and implement the interface IJsonTokenConverter using your favorite DI framework.  The converter has just one method `TConfiguration Convert<TConfiguration>(JToken token)`.

## Load JSON from other media

To load `JToken` from other storages than the file system just implement the interface `IConfigurationLoader<JToken>`. For example, if the JSON configuration are in a database then inject the database context or a data access layer and select corresponding data rows.

## Use different data models

If you are using other data model than JSON then reference the package `<<Thinktecture.Configuration>>` and implement the interfaces `IConfigurationLoader<JToken>`, `IConfigurationProvider<TRawData>` and  `IConfigurationSelector<TRawData>`. It sounds like much but if you look into the code of the corresponding JSON-based classes you will see that the classes are pretty small and trivial.
