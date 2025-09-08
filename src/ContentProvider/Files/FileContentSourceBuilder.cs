// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

namespace ContentProvider.Files;

public sealed class FileContentSourceBuilder
    : ContentSourceOptionsBuilder<FileContentSourceBuilder,  FileContentSource, FileContentSourceOptions>
{
    private readonly string _baseDirectory;

    internal FileContentSourceBuilder(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
    }

    public override FileContentSource Build() => new(_baseDirectory, Options);

    public FileContentSourceBuilder WithSearchPattern(string searchPattern)
    {
        Options.SearchPattern = searchPattern;
        return this;
    }

    public FileContentSourceBuilder IncludeAllDirectoriesRecursively()
    {
        Options.SearchOption = SearchOption.AllDirectories;
        return this;
    }

    public FileContentSourceBuilder IncludeTopDirectoryOnly()
    {
        Options.SearchOption = SearchOption.TopDirectoryOnly;
        return this;
    }

    public FileContentSourceBuilder WithSearchOption(SearchOption searchOption)
    {
        Options.SearchOption = searchOption;
        return this;
    }

    public FileContentSourceBuilder KeepExtensions()
    {
        Options.KeepExtension = true;
        return this;
    }
}
