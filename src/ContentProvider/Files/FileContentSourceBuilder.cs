// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.Files;

namespace ContentProvider;

public sealed class FileContentSourceBuilder
{
    private readonly string _baseDirectory;
    private readonly FileContentSourceOptions _options = new();

    internal FileContentSourceBuilder(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
    }

    internal FileContentSource Build() => new(_baseDirectory, _options);

    public FileContentSourceBuilder WithSearchPattern(string searchPattern)
    {
        _options.SearchPattern = searchPattern;
        return this;
    }

    public FileContentSourceBuilder WithSearchOption(SearchOption searchOption)
    {
        _options.SearchOption = searchOption;
        return this;
    }

    public FileContentSourceBuilder KeepExtensions()
    {
        _options.KeepExtension = true;
        return this;
    }

    public FileContentSourceBuilder WithNameTransformer(Func<string, string> nameTransformer)
    {
        _options.NameTransformer = nameTransformer;
        return this;
    }
}
