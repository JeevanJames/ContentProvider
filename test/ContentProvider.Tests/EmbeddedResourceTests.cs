﻿#region --- License & Copyright Notice ---
/*
ContentProvider Framework
Copyright 2020 Jeevan James

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

using System.Threading.Tasks;

using ContentProvider.EmbeddedResources;

using Shouldly;

using Xunit;

namespace ContentProvider.Tests
{
    public sealed class EmbeddedResourceTests
    {
        [Fact]
        public async Task Able_to_load_embedded_resources()
        {
            ContentManager.Register("Text", new ContentBuilder()
                .From.ResourcesInExecutingAssembly(rootNamespace: "ContentProvider.Tests")
                .Build());

            ContentSet content = ContentManager.Get("Text");
            string value = await content.Get("Content.txt")
                .ConfigureAwait(false);

            content.ShouldNotBeNull();
            value.ShouldBe("This is the content.");
        }
    }
}