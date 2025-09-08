// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Reflection;

namespace ContentProvider.EmbeddedResources;

public sealed class ResourceContentSourceOptionsBuilder
    : ContentSourceOptionsBuilder<ResourceContentSourceOptionsBuilder,  ResourceContentSource, ResourceContentSourceOptions>
{
    private readonly Assembly _assembly;

    internal ResourceContentSourceOptionsBuilder(Assembly assembly)
    {
        _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    }

    public override ResourceContentSource Build() => new(_assembly, Options);

    public ResourceContentSourceOptionsBuilder WithFileExtension(string extension)
    {
        Options.FileExtension = extension;
        return this;
    }

    public ResourceContentSourceOptionsBuilder WithRootNamespace(string rootNamespace)
    {
        Options.RootNamespace = rootNamespace;
        return this;
    }

    public ResourceContentSourceOptionsBuilder WithRootNamespace(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        Options.RootNamespace = type.Namespace;
        return this;
    }

    public ResourceContentSourceOptionsBuilder WithRootNamespace<T>()
    {
        Options.RootNamespace = typeof(T).Namespace;
        return this;
    }

    public ResourceContentSourceOptionsBuilder KeepExtensions()
    {
        Options.KeepExtension = true;
        return this;
    }
}
