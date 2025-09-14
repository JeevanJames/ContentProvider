# Content Provider

The ContentProvider framework provides a mechanism for retrieving string and binary resources from various sources such as files, embedded resources, etc.

[![Build status](https://ci.appveyor.com/api/projects/status/v5yyr0kpm7m4jdim?svg=true)](https://ci.appveyor.com/project/JeevanJames/contentprovider) [![Test status](https://img.shields.io/appveyor/tests/JeevanJames/contentprovider.svg)](https://ci.appveyor.com/project/JeevanJames/contentprovider/build/tests)

[[_TOC_]]

## NuGet packages

Name | Links   | Description
-----|---------|------------
ContentProvider | [![NuGet Version](https://img.shields.io/nuget/v/ContentProvider.svg?style=flat)](https://www.nuget.org/packages/ContentProvider/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ContentProvider.svg)](https://www.nuget.org/packages/ContentProvider/) | Core package.
ContentProvider.Extensions.DependencyInjection | [![NuGet Version](https://img.shields.io/nuget/v/ContentProvider.Extensions.DependencyInjection.svg?style=flat)](https://www.nuget.org/packages/ContentProvider.Extensions.DependencyInjection/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ContentProvider.Extensions.DependencyInjection.svg)](https://www.nuget.org/packages/ContentProvider.Extensions.DependencyInjection/) | Additional support dependency injection based on `Microsoft.Extensions.DependencyInjection`.
ContentProvider.Formats.Json | [![NuGet Version](https://img.shields.io/nuget/v/ContentProvider.Formats.Json.svg?style=flat)](https://www.nuget.org/packages/ContentProvider.Formats.Json/) [![NuGet Downloads](https://img.shields.io/nuget/dt/ContentProvider.Formats.Json.svg)](https://www.nuget.org/packages/ContentProvider.Formats.Json/) | Extensions to support JSON data.

## Getting started

The first step is to register one or more content sets. A content set represents a set of data that can be retrieved from one or more content sources, such as a file, embedded resource, REST API, etc.

The framework provides two ways to register content sets, depending on the type of application:

* `ContentManager`: This class is provided in the core `ContentProvider` library. You will typically use the predefined singleton instance `ContentManager.Global`. Creating new instances is only for advanced scenarios and mocking during testing.

    `ContentManager` provides multiple `Register` method overloads to register content sets.

    To retrieve content sets later on, use the `GetContentSet` method overloads provided on the same `ContentManager` instance used to register them.

* As dependency injection services: The `ContentProvider.Extensions.DependencyInjection` library provides `AddContent` extensions on `IServiceCollection` to register content sets as injectable services.

    To retrieve content sets later on, use dependency injection.

### Registering a named content set

A named content set is an untyped content set and is represented by the `IContentSet` interface that provides core methods to retrieve content.

```cs
// Using ContentManager.Global
ContentManager.Register("DbScripts", builder => builder
    .From.FilesInCurrentDirectory(o => o.WithFileExtension("sql"))
    .ThenFrom.ResourcesInExecutingAssembly(o => o.WithFileExtension("sql")));

// Using IServiceCollection
services.AddContent("DbScripts", builder => builder
    .From.FilesInCurrentDirectory(o => o.WithFileExtension("sql"))
    .ThenFrom.ResourcesInExecutingAssembly(o => o.WithFileExtension("sql").WithResourceNamespace("MyNamespace")));
```

Retrieving a named content set will get you a `IContentSet` instance.

```cs
// Using ContentManager.Global
IContentSet dbScripts = ContentManager.Global.GetContentSet("DbScripts");

// Using dependency injection (IServiceProvider)
IContentSet dbScripts = serviceProvider.GetRequiredKeyedService<IContentSet>("DbScripts");

// Using dependency injection (constructor)
public MyClassCtor([FromKeyedServices("DbScripts")] IContentSet dbScripts) {}
```

### Registering a type content set

A typed content set represents a custom content set class that derives from `ContentSet`. It can be used to avoid the magic string approach of named content sets and also provide strongly-typed methods to access content (discussed later).

```cs
// Simple type content set example
public sealed class DbScriptsContentSet : ContentSet;
```

Registering a type content set:

```cs
// Using ContentManager.Global
ContentManager.Register<DbScriptsContentSet>(builder => builder
    .From.FilesAndResources(o => o
        .WithFileExtension("sql")
        .WithResourceNamespace<DbScriptsContentSet>()));

// Using IServiceCollection
services.AddContent(<DbScriptsContentSet>(builder => builder
    .From.FilesAndResources(o => o
        .WithFileExtension("sql")
        .WithResourceNamespace<DbScriptsContentSet>()));
```

Retrieving a typed content set will get you an instance of the custom content set class.

```cs
// Using ContentManager.Global
DbScriptsContentSet dbScripts = ContentManager.Global.GetContentSet<DbScripts>();

// Using dependency injection (IServiceProvider)
DbScriptsContentSet dbScripts = serviceProvider.GetRequiredService<DbScriptsContentSet>();

// Using dependency injection (constructor)
public MyClassCtor(DbScriptsContentSet dbScripts) {}
```

### Registering a named type content set

TBD

## Content sources

TBD

### Built-in content sources

TBD

### Combining file and resource content sources

TBD

### Creating a content source

TBD

## Typed content sets

TBD
