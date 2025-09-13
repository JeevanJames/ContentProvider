// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Reflection;

using ContentProvider.FilesAndResources;

namespace ContentProvider.EmbeddedResources;

public sealed class ResourceContentSourceOptionsBuilder
    : BaseFileAndResourceContentSourceOptionsBuilder<ResourceContentSourceOptionsBuilder,  ResourceContentSource, ResourceContentSourceOptions>
{
    private readonly Assembly _assembly;

    internal ResourceContentSourceOptionsBuilder(Assembly assembly)
    {
        _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
    }

    public override ResourceContentSource Build() => new(_assembly, Options);

    public ResourceContentSourceOptionsBuilder WithResourceNamespace(string rootNamespace)
    {
        Options.ResourceNamespace = rootNamespace;
        return this;
    }

    public ResourceContentSourceOptionsBuilder WithResourceNamespace(Type type)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        Options.ResourceNamespace = type.Namespace;
        return this;
    }

    public ResourceContentSourceOptionsBuilder WithResourceNamespace<T>()
    {
        Options.ResourceNamespace = typeof(T).Namespace;
        return this;
    }
}
