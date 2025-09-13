// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using ContentProvider.FilesAndResources;

namespace ContentProvider.Files;

public sealed class FileContentSourceOptionsBuilder
    : BaseFileAndResourceContentSourceOptionsBuilder<FileContentSourceOptionsBuilder,  FileContentSource, FileContentSourceOptions>
{
    private readonly string _baseDirectory;

    internal FileContentSourceOptionsBuilder(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
    }

    public override FileContentSource Build() => new(_baseDirectory, Options);
}
