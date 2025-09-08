// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Reflection;

using ContentProvider.EmbeddedResources;

namespace ContentProvider;

public sealed class ResourceContentSourceBuilder
{
    private readonly Assembly _assembly;
    private readonly ResourceContentSourceOptions _options = new();

    internal ResourceContentSourceBuilder(Assembly assembly)
    {
        _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    }

    internal ResourceContentSource Build() => new(_assembly, _options);

    public ResourceContentSourceBuilder WithFileExtension(string extension)
    {
        _options.FileExtension = extension;
        return this;
    }

    public ResourceContentSourceBuilder WithRootNamespace(string rootNamespace)
    {
        _options.RootNamespace = rootNamespace;
        return this;
    }

    public ResourceContentSourceBuilder WithRootNamespace(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        _options.RootNamespace = type.Namespace;
        return this;
    }

    public ResourceContentSourceBuilder WithRootNamespace<T>()
    {
        _options.RootNamespace = typeof(T).Namespace;
        return this;
    }

    public ResourceContentSourceBuilder KeepExtensions()
    {
        _options.KeepExtension = true;
        return this;
    }
}
