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

public sealed class FileContentSourceOptions : ContentSourceOptions
{
    /// <summary>
    ///     Gets or sets the search string to match against the names of the files. This parameter
    ///     can contain a combination of valid literal path and wildcard (* and ?) characters, but it
    ///     doesn't support regular expressions.
    ///     <para/>
    ///     The default is <c>*</c>.
    /// </summary>
    public string SearchPattern { get; set; } = "*";

    /// <summary>
    ///     Gets or sets an enumeration value that specified whether the search operation should
    ///     include all subdirectories or only the current directory.
    /// </summary>
    public SearchOption SearchOption { get; set; } = SearchOption.AllDirectories;

    /// <summary>
    ///     Gets or sets a value indicating whether to keep the file extension in the content name.
    /// </summary>
    public bool KeepExtension { get; set; }
}
