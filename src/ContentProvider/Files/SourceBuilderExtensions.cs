#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright (c) 2020-2024 Damian Kulik, Jeevan James

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

namespace ContentProvider.Files;

public static class SourceBuilderExtensions
{
    /// <summary>
    ///     Registers a <see cref="ContentSource"/> for content from files on the file system.
    /// </summary>
    /// <param name="builder">The <see cref="ContentSourceBuilder"/> instance.</param>
    /// <param name="baseDirectory">The base directory to retrieve the files from.</param>
    /// <param name="options">Options for registering the content source.</param>
    /// <returns>An instance of <see cref="ContentBuilder"/>.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="builder"/> or <paramref name="options"/> parameters are
    ///     <c>null</c>.
    /// </exception>
    public static ContentBuilder FilesIn(this ContentSourceBuilder builder, string baseDirectory,
        FileContentSourceOptions options)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));
        if (options is null)
            throw new ArgumentNullException(nameof(options));

        return builder.Source(new FileContentSource(baseDirectory, options));
    }
}
