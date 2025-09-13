// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;

namespace ContentProvider.FilesAndResources;

#pragma warning disable CA1005 // Avoid excessive parameters on generic types
public abstract class BaseFileAndResourceContentSourceOptionsBuilder<TSelf, TContentSource, TContentSourceOptions> :
    ContentSourceOptionsBuilder<TSelf, TContentSource, TContentSourceOptions>
    where TSelf : BaseFileAndResourceContentSourceOptionsBuilder<TSelf, TContentSource, TContentSourceOptions>
    where TContentSource : ContentSource
    where TContentSourceOptions : BaseFileAndResourceContentSourceOptions, new()
{
    public TSelf FilterNamesBy(Func<string, bool> nameFilter)
    {
        if (nameFilter is null)
            throw new ArgumentNullException(nameof(nameFilter));
        Options.NameFilter = nameFilter;
        return (TSelf)this;
    }

    public TSelf FilterNamesBy(Regex namePattern)
    {
        if (namePattern is null)
            throw new ArgumentNullException(nameof(namePattern));
        return FilterNamesBy(namePattern.IsMatch);
    }

    public TSelf WithFileExtension(string fileExtension)
    {
        if (string.IsNullOrWhiteSpace(fileExtension))
            throw new ArgumentException("File extension cannot be null or whitespace.", nameof(fileExtension));
        Options.FileExtension = fileExtension;
        return (TSelf)this;
    }

    public TSelf KeepExtensions()
    {
        Options.KeepExtension = true;
        return (TSelf)this;
    }
}
