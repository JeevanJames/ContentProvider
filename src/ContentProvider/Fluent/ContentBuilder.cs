// Copyright (c) 2020-2025 Damian Kulik, Jeevan James
// Licensed under the Apache License, Version 2.0.  See LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ContentProvider;

public sealed class ContentBuilder : Collection<ContentSource>
{
    public ContentSourceBuilder From => new ContentSourceBuilder(this);

    public ContentSource[] Build() => this.ToArray();
}
