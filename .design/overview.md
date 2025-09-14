# Overview

There are three primary projects:
* `ContentProvider`: Core library
* `ContentProvider.Extensions.DependencyInjection`: Extensions to add support for the .NET Core hosting model and dependency injection.
* `ContentProvider.Formats.Json`: Provides extensions for reading the content in various JSON forms.

## `ContentProvider`

There are 3 primary classes:
* `ContentManager`: The central class, used to register content sources and retrieve content sets.
* `ContentSource`: Represents a source of content. One or more content sources can be registered with the `ContentManager`.
* `ContentSet`: Facade to retrieve content from a registered set of sources.

### `ContentManager`

`ContentManager` provides `Register` methods to register one or more content sources under a unique name. The content source collection is called a `ContentSet`. When retrieving content, the content sources are searched in the order they were registered, providing a fallback mechanism.

To retrieve content, `ContentManager` provides a `GetContentSet` method to retrieve a `ContentSet` by it's unique name.

For simple scenarios, `ContentManager` also provides a predefined static instance `Global` that can be used globally without needing to create an instance.

### `ContentSource` and `ContentSource<TOptions>`

A content source represents a source for the content, such as a file or embedded resource. It contains the logic to be able to read the content, either as a string or as binary (`byte[]`).

Most content sources need to be configured. For example, file content sources need to be configured for the base directory, file name patterns, etc. Configurable content sources derive from `ContentSource<TOption>`. The `TOptions` generic parameter is any type that derives from `ContentSourceOptions`. An options class that derives from `ContentSourceOptions` can have any properties in any structure needed to configure the content source.

A content source can retrieve one or more contents. For example, a file content source can contain the content from multiple files in a directory. Each content in a content source should be indexed by a unique key. The same unique key can be present in multiple content sets, providing the fallback mechanism.

## `ContentProvider.Extensions.DependencyInjection`

The `ContentProvider.Extensions.DependencyInjection` project provides extensions to work with the .NET Core hosting model. It does this by providing extension methods on `IServiceCollection` to register content sources with the dependency injection container.

## `ContentProvider.Formats.Json`

TBD
