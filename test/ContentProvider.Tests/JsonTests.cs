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
using System.Threading.Tasks;

using ContentProvider.Formats.Json;
using ContentProvider.Formats.Json.Structures;
using ContentProvider.Tests.Fixtures;

using Shouldly;

using Xunit;

namespace ContentProvider.Tests
{
    [Collection("ContentManager")]
    public sealed class JsonTests
    {
        private readonly IContentSet _contentSet;

        public JsonTests(ContentManagerFixture fixture)
        {
            if (fixture is null)
                throw new ArgumentNullException(nameof(fixture));
            _contentSet = fixture.ContentManager.GetContentSet("Json");
        }

        [Fact]
        public async Task Able_to_load_json_resources()
        {
            var value = await _contentSet.GetAsJsonAsync<JsonStruct>("Content")
                .ConfigureAwait(false);

            value.ShouldNotBeNull();
        }

        [Fact]
        public async Task Able_to_load_custom_list_entry()
        {
            var value = await _contentSet.GetJsonAsCustomListEntry<CustomListEntry>("CustomListEntry", "Flash", 2)
                .ConfigureAwait(false);

            value.ShouldNotBeNull();
            value.Name.ShouldBe("Barry Allen");
        }
    }

    public sealed class JsonStruct
    {
        public string Name { get; set; } = null!;

        public string City { get; set; } = null!;
    }

    public sealed class CustomListEntry
    {
        public string Name { get; set; } = null!;
    }
}
