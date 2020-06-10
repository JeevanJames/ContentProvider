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
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContentProvider.Formats.Json
{
    public static class ContentSetExtensions
    {
        public static Task<T> GetAsJsonAsync<T>(this IContentSet contentSet,
            string name,
            JsonSerializerOptions? serializerOptions = null)
        {
            if (contentSet is null)
                throw new ArgumentNullException(nameof(contentSet));

            return contentSet.GetAsJsonAsyncInternal<T>(name, serializerOptions);
        }

        private static async Task<T> GetAsJsonAsyncInternal<T>(this IContentSet contentSet,
            string name,
            JsonSerializerOptions? serializerOptions = null)
        {
            string json = await contentSet.GetAsStringAsync(name).ConfigureAwait(false);

            using var ms = new MemoryStream();
#pragma warning disable CA2000 // Dispose objects before losing scope
            var writer = new StreamWriter(ms, Encoding.UTF8);
#pragma warning restore CA2000 // Dispose objects before losing scope
            writer.Write(json);
            writer.Flush();
            ms.Position = 0;

            return await JsonSerializer.DeserializeAsync<T>(ms, serializerOptions ?? JsonOptions.SerializerOptions)
                .ConfigureAwait(false);
        }

        public static T GetAsJson<T>(this IContentSet contentSet, string name,
            JsonSerializerOptions? serializerOptions = null)
        {
            if (contentSet is null)
                throw new ArgumentNullException(nameof(contentSet));

            string json = contentSet.GetAsString(name);
            return JsonSerializer.Deserialize<T>(json, serializerOptions ?? JsonOptions.SerializerOptions);
        }
    }
}
