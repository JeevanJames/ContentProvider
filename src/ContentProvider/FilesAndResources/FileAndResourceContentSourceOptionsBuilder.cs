// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider.FilesAndResources;

public sealed class FileAndResourceContentSourceOptionsBuilder
{
    private FileAndResourceContentSourceOptions? _options;

    public FileAndResourceContentSourceOptions Build() => _options ?? FileAndResourceContentSourceOptions.Default;

    public FileAndResourceContentSourceOptionsBuilder WithNameTransformer(Func<string, string> nameTransformer)
    {
        _options ??= new FileAndResourceContentSourceOptions();
        _options.NameTransformer = nameTransformer;
        return this;
    }

    public FileAndResourceContentSourceOptionsBuilder MatchNames(Func<string, bool>? nameMatcher)
    {
        _options ??= new FileAndResourceContentSourceOptions();
        _options.NameMatcher = nameMatcher;
        return this;
    }

    public FileAndResourceContentSourceOptionsBuilder WithFileExtension(string fileExtension)
    {
        _options ??= new FileAndResourceContentSourceOptions();
        _options.FileExtension = fileExtension;
        return this;
    }

    public FileAndResourceContentSourceOptionsBuilder KeepExtensions()
    {
        _options ??= new FileAndResourceContentSourceOptions();
        _options.KeepExtension = true;
        return this;
    }

    public FileAndResourceContentSourceOptionsBuilder WithResourceNamespace(string resourceNamespace)
    {
        _options ??= new FileAndResourceContentSourceOptions();
        _options.ResourceNamespace = resourceNamespace;
        return this;
    }

    public FileAndResourceContentSourceOptionsBuilder WithResourceNamespace(Type namespaceType)
    {
        if (namespaceType is null)
            throw new ArgumentNullException(nameof(namespaceType));

        _options ??= new FileAndResourceContentSourceOptions();
        _options.ResourceNamespace = namespaceType.Namespace;
        return this;
    }

    public FileAndResourceContentSourceOptionsBuilder WithResourceNamespace<T>() =>
        WithResourceNamespace(typeof(T));
}
