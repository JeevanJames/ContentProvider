#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright (c) 2020 Damian Kulik, Jeevan James

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ContentProvider.Files
{
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
                .Select(name => Options.NameTransformer is null ? name : Options.NameTransformer(name))
                .ToList();
        }

        public async override Task<(bool success, string? content)> TryLoadAsString(string name)
        {
            string file = _files.Find(file => file.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (file is null)
                return (false, null);

            string filePath = Path.Combine(_baseDirectory, file);

#pragma warning disable S1854 // Unused assignments should be removed
#if NETSTANDARD2_0
            string content;
            using (var reader = new StreamReader(filePath))
                content = await reader.ReadToEndAsync().ConfigureAwait(false);
#else
            string content = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
#endif
#pragma warning restore S1854 // Unused assignments should be removed

            return (true, content);
        }

        public async override Task<(bool success, byte[]? content)> TryLoadAsBinary(string name)
        {
            string file = _files.Find(file => file.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (file is null)
                return (false, null);

            string filePath = Path.Combine(_baseDirectory, file);

#pragma warning disable S1854 // Unused assignments should be removed
#if NETSTANDARD2_0
            using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var ms = new MemoryStream();
            await fs.CopyToAsync(ms).ConfigureAwait(false);
            byte[] content = ms.ToArray();
            return (true, content);
#else
            byte[] content = await File.ReadAllBytesAsync(filePath).ConfigureAwait(false);
            return (true, content);
#endif
#pragma warning restore S1854 // Unused assignments should be removed
        }
    }
}
