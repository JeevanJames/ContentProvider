// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Globalization;

namespace ContentProvider.Files;

[DebuggerDisplay("File content source: {_baseDirectory} ({_files.Count} items)")]
public sealed class FileContentSource : ContentSource<FileContentSourceOptions>
{
    private readonly string _baseDirectory;
    private readonly List<string> _files;

    public FileContentSource(string baseDirectory, FileContentSourceOptions options)
        : base(options)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
            throw new ArgumentException(Errors.FilesInvalidBaseDirectory, nameof(baseDirectory));
        if (!Directory.Exists(baseDirectory))
            throw new DirectoryNotFoundException(string.Format(CultureInfo.CurrentCulture, Errors.FilesMissingBaseDirectory, baseDirectory));

        _baseDirectory = Path.GetFullPath(baseDirectory);
        _files = Directory.EnumerateFiles(_baseDirectory, Options.SearchPattern, Options.SearchOption)
            .Select(path => path.Substring(_baseDirectory.Length + 1))
            .Select(name =>
            {
                string contentName = Options.NameTransformer is null ? name : Options.NameTransformer(name);
                if (!Options.KeepExtension)
                {
                    int dotIndex = contentName.LastIndexOf('.');
                    if (dotIndex > 0)
                        contentName = contentName.Substring(0, dotIndex);
                }

                return contentName;
            })
            .ToList();
    }

    public override async Task<string?> LoadAsStringAsync(string name)
    {
        string file = _files.Find(file => file.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (file is null)
            return null;

        string filePath = Path.Combine(_baseDirectory, file);

        using var reader = new StreamReader(filePath);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public override async Task<byte[]?> LoadAsBinaryAsync(string name)
    {
        string file = _files.Find(file => file.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (file is null)
            return null;

        string filePath = Path.Combine(_baseDirectory, file);

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var ms = new MemoryStream();
        await fs.CopyToAsync(ms).ConfigureAwait(false);
        return ms.ToArray();
    }

    public override string? LoadAsString(string name)
    {
        string file = _files.Find(file => file.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (file is null)
            return null;

        string filePath = Path.Combine(_baseDirectory, file);

        using var reader = new StreamReader(filePath);
        return reader.ReadToEnd();
    }

    public override byte[]? LoadAsBinary(string name)
    {
        string file = _files.Find(file => file.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (file is null)
            return null;

        string filePath = Path.Combine(_baseDirectory, file);

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var ms = new MemoryStream();
        fs.CopyToAsync(ms);
        return ms.ToArray();
    }
}
