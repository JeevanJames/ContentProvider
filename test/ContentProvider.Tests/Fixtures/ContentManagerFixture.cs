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

using ContentProvider.EmbeddedResources;
using ContentProvider.Tests.Content;
using Xunit;

namespace ContentProvider.Tests.Fixtures;

public sealed class ContentManagerFixture
{
    public ContentManagerFixture()
    {
        ContentManager = new ContentManager()
            .Register("Text", builder => builder
                .From.ResourcesInExecutingAssembly(new EmbeddedResourceContentSourceOptions
                {
                    FileExtension = "txt",
                    RootNamespace = typeof(TextContentSet).Namespace,
                }))
            .Register("Json", builder => builder
                .From.ResourcesInExecutingAssembly(new EmbeddedResourceContentSourceOptions
                {
                    FileExtension = "json",
                    RootNamespace = typeof(JsonContentSet).Namespace,
                }));
    }

    public ContentManager ContentManager { get; }
}

[CollectionDefinition("ContentManager")]
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public sealed class ContentManagerFixtureCollection : ICollectionFixture<ContentManagerFixture>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
{
}
