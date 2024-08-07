﻿#region --- License & Copyright Notice ---
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

namespace ContentProvider;

/// <summary>
///     Base class for all content source options.
/// </summary>
public abstract class ContentSourceOptions
{
    /// <summary>
    ///     Gets or sets an optional delegate that can be used to transform the content entry names
    ///     generated from the content source.
    /// </summary>
    public Func<string, string>? NameTransformer { get; set; }
}
