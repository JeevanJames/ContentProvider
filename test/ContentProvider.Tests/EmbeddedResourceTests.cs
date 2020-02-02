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

using Shouldly;

using Xunit;

namespace ContentProvider.Tests
{
    [Collection("Content")]
    public sealed class EmbeddedResourceTests
    {
#pragma warning disable CA1801 // Unused parameter.
#pragma warning disable RCS1163 // Unused parameter.
#pragma warning disable IDE0060 // Remove unused parameter
        public EmbeddedResourceTests(ContentFixture fixture)
#pragma warning restore IDE0060 // Remove unused parameter
#pragma warning restore RCS1163 // Unused parameter.
#pragma warning restore CA1801 // Unused parameter.
        {
        }

        [Fact]
        public async Task Able_to_load_embedded_resources()
        {
            ContentSet content = ContentManager.Get("Text");
            string value = await content.GetAsString("Content.txt")
                .ConfigureAwait(false);

            content.ShouldNotBeNull();
            value.ShouldBe("This is the content.");
        }
    }
}
