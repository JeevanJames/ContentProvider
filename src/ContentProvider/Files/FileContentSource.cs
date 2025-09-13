// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace ContentProvider.Files;

[DebuggerDisplay("File content source: {_baseDirectory})")]
public sealed class FileContentSource : ContentSource<FileContentSourceOptions>
{
    private readonly string _baseDirectory;

    public FileContentSource(string baseDirectory, FileContentSourceOptions options) : base(options)
    {
        if (string.IsNullOrWhiteSpace(baseDirectory))
            throw new ArgumentException(Errors.FilesInvalidBaseDirectory, nameof(baseDirectory));
        _baseDirectory = Path.GetFullPath(baseDirectory);
    }

    public override async Task<string?> LoadAsStringAsync(string name)
    {
        if (!TryFindFile(name, out string? filePath))
            return null;

        using var reader = new StreamReader(filePath);
        return await reader.ReadToEndAsync().ConfigureAwait(false);
    }

    public override async Task<byte[]?> LoadAsBinaryAsync(string name)
    {
        if (!TryFindFile(name, out string? filePath))
            return null;

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var ms = new MemoryStream();
        await fs.CopyToAsync(ms).ConfigureAwait(false);
        return ms.ToArray();
    }

    public override string? LoadAsString(string name)
    {
        if (!TryFindFile(name, out string? filePath))
            return null;

        using var reader = new StreamReader(filePath);
        return reader.ReadToEnd();
    }

    public override byte[]? LoadAsBinary(string name)
    {
        if (!TryFindFile(name, out string? filePath))
            return null;

        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var ms = new MemoryStream();
        fs.CopyToAsync(ms);
        return ms.ToArray();
    }

    // We do not cache any file details on startup. Each request for file content is dynamic and always
    // queries the directory. This allows 
    private bool TryFindFile(string name, out string? filePath)
    {
        filePath = null;

        if (!Directory.Exists(_baseDirectory))
            return false;

        IEnumerable<string> files = Directory.EnumerateFiles(_baseDirectory, "*", SearchOption.TopDirectoryOnly);
        foreach (string file in files)
        {
            string resolvedName = Options.KeepExtension ? Path.GetFileName(file) : Path.GetFileNameWithoutExtension(file);
            if (Options.NameFilter?.Invoke(resolvedName) == false)
                continue;
            if (Options.NameTransformer is not null)
                resolvedName = Options.NameTransformer(resolvedName);
            if (resolvedName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                filePath = file;
                return true;
            }
        }

        return false;
    }
}
