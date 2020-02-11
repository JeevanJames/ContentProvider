#region --- License & Copyright Notice ---
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

using ContentProvider.Formats.Json;
using ContentProvider.Formats.Json.Structures;

using Shouldly;

using Xunit;

namespace ContentProvider.Tests
{
    [Collection("Content")]
    public sealed class JsonTests
    {
        private readonly ContentSet _contentSet;

#pragma warning disable CA1801 // Unused parameter.
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
        public JsonTests(ContentFixture fixture)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore CA1801 // Unused parameter.
        {
            _contentSet = ContentManager.Get("Json");
        }

        [Fact]
        public async Task Able_to_load_json_resources()
        {
            var value = await _contentSet.GetAsJson<JsonStruct>("Content.json")
                .ConfigureAwait(false);

            value.ShouldNotBeNull();
        }

        [Fact]
        public async Task Able_to_load_custom_list_entry()
        {
            var value = await _contentSet.GetJsonAsCustomListEntry<CustomListEntry>("CustomListEntry.json", "Flash", 2)
                .ConfigureAwait(false);

            value.ShouldNotBeNull();
            value.Name.ShouldBe("Barry Allen");
        }
    }

    public sealed class JsonStruct
    {
        public string Name { get; set; }

        public string City { get; set; }
    }

    public sealed class CustomListEntry
    {
        public string Name { get; set; }
    }
}
