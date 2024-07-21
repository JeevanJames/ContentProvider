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

using System.Diagnostics;

namespace ContentProvider;

[DebuggerDisplay("{_contentSets.Count} registered content sets")]
public sealed partial class ContentManager : IContentManager
{
    private readonly Dictionary<string, ContentSet> _contentSets = new(StringComparer.OrdinalIgnoreCase);

    public ContentManager Register(string name, params ContentSource[] sources) =>
        Register<ContentSet>(name, sources);

    public ContentManager Register(string name, Action<ContentBuilder> builderSetup)
    {
        if (builderSetup is null)
            throw new ArgumentNullException(nameof(builderSetup));

        var builder = new ContentBuilder();
        builderSetup(builder);
        ContentSource[] sources = builder.Build();

        return Register<ContentSet>(name, sources);
    }

    public ContentManager Register<TContentSet>(params ContentSource[] sources)
        where TContentSet : ContentSet, new() =>
        Register<TContentSet>(typeof(TContentSet).AssemblyQualifiedName, sources);

    public ContentManager Register<TContentSet>(Action<ContentBuilder> builderSetup)
        where TContentSet : ContentSet, new()
    {
        if (builderSetup is null)
            throw new ArgumentNullException(nameof(builderSetup));

        var builder = new ContentBuilder();
        builderSetup(builder);
        ContentSource[] sources = builder.Build();

        return Register<TContentSet>(typeof(TContentSet).AssemblyQualifiedName, sources);
    }

    private ContentManager Register<TContentSet>(string name, ContentSource[] sources)
        where TContentSet : ContentSet, new()
    {
        if (name is null)
            throw new ArgumentNullException(nameof(name));
        if (sources is null)
            throw new ArgumentNullException(nameof(sources));

        var contentSet = new TContentSet
        {
            Name = name,
        };
        foreach (ContentSource source in sources)
            contentSet.Sources.Add(source);
        _contentSets.Add(name, contentSet);
        return this;
    }

    /// <inheritdoc/>
    public IContentSet GetContentSet(string name)
    {
        return _contentSets.TryGetValue(name, out ContentSet contentSet)
            ? contentSet
            : throw new ArgumentException($"Could not find a content set named {name}.", nameof(name));
    }

    /// <inheritdoc/>
    public TContentSet GetContentSet<TContentSet>()
        where TContentSet : ContentSet, new()
    {
        if (!_contentSets.TryGetValue(typeof(TContentSet).AssemblyQualifiedName, out ContentSet contentSet))
            throw new ArgumentException($"Could not find a content set typed {typeof(TContentSet).FullName}.");

        if (contentSet is TContentSet typedContentSet)
            return typedContentSet;

        throw new ContentException($"The content set for type {typeof(TContentSet).FullName} is a different type {contentSet.GetType().FullName}.");
    }
}

public sealed partial class ContentManager
{
    /// <summary>
    ///     Gets a pre-instatiated global instance of <see cref="ContentManager"/>.
    /// </summary>
    public static ContentManager Global { get; } = new ContentManager();
}
