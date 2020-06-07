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
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContentProvider.Formats.Json.Structures
{
    public static class ContentSetExtensions
    {
        public static async Task<IEnumerable<T>> GetJsonAsList<T>(this IContentSet contentSet, string name,
            Func<T, bool>? predicate = null, JsonSerializerOptions? serializerOptions = null)
        {
            IEnumerable<T> value = await contentSet.GetAsJson<IEnumerable<T>>(name, serializerOptions)
                .ConfigureAwait(false);
            if (predicate != null)
                value = value.Where(predicate);
            return value;
        }

        public static Task<T> GetJsonAsListEntry<T>(this IContentSet contentSet,
            string name,
            Func<T, bool> predicate,
            JsonSerializerOptions? serializerOptions = null)
        {
            if (predicate is null)
                throw new ArgumentNullException(nameof(predicate));

            return contentSet.GetJsonAsListEntryInternal(name, predicate, serializerOptions);
        }

        private static async Task<T> GetJsonAsListEntryInternal<T>(this IContentSet contentSet,
            string name,
            Func<T, bool> predicate,
            JsonSerializerOptions? serializerOptions = null)
        {
            IEnumerable<T> collection = await contentSet.GetAsJson<IEnumerable<T>>(name, serializerOptions)
                .ConfigureAwait(false);
            T result = collection.FirstOrDefault(predicate);
            return result;
        }

        public static Task<T?> GetJsonAsCustomListEntry<T>(this IContentSet contentSet,
            string name,
            params object[] args)
            where T : class
        {
            if (contentSet is null)
                throw new ArgumentNullException(nameof(contentSet));

            //IReadOnlyList<Type> argTypes = args.Select(arg => arg.GetType()).ToList();

            //TODO: Check all arg types are valid JSON types - number, string, boolean

            return contentSet.GetJsonAsCustomListEntryInternal<T>(name, args);
        }

        private static async Task<T?> GetJsonAsCustomListEntryInternal<T>(this IContentSet contentSet,
            string name,
            params object[] args)
            where T : class
        {
            string json = await contentSet.GetAsStringAsync(name).ConfigureAwait(false);

            JsonDocument doc = JsonDocument.Parse(json, new JsonDocumentOptions
            {
                AllowTrailingCommas = true,
                CommentHandling = JsonCommentHandling.Skip,
            });

            foreach (JsonElement item in doc.RootElement.EnumerateArray())
            {
                if (!item.TryGetProperty("Inputs", out JsonElement inputsProperty))
                    throw new JsonException($"Cannot find Inputs property in the JSON item {item}.");
                if (inputsProperty.ValueKind != JsonValueKind.Array)
                    throw new JsonException($"The Inputs property must be an array in the JSON item {item}.");
                if (inputsProperty.GetArrayLength() != args.Length)
                    throw new JsonException($"Invalid number of arguments in the Inputs property for the JSON item {item}.");

                bool inputsMatch = true;
                List<JsonElement> inputArgs = inputsProperty.EnumerateArray().ToList();
                for (var i = 0; i < inputArgs.Count; i++)
                {
                    if (!inputArgs[i].ToString().Equals(args[i].ToString(), StringComparison.Ordinal))
                    {
                        inputsMatch = false;
                        break;
                    }
                }

                if (inputsMatch)
                {
                    if (!item.TryGetProperty("Data", out JsonElement dataProperty))
                        throw new JsonException($"Cannot find Data property in the JSON item {item}.");
                    T value = JsonSerializer.Deserialize<T>(dataProperty.GetRawText(), JsonOptions.SerializerOptions);
                    return value;
                }
            }

            return default;
        }

        public static async Task<IDictionary<string, T>> GetJsonAsDictionary<T>(this IContentSet contentSet,
            string name,
            JsonSerializerOptions? serializerOptions = null)
        {
            return await contentSet.GetAsJson<IDictionary<string, T>>(name, serializerOptions).ConfigureAwait(false);
        }

        public static async Task<T> GetJsonAsDictionaryEntry<T>(this IContentSet contentSet,
            string name,
            string key,
            StringComparison keyComparison = StringComparison.Ordinal,
            JsonSerializerOptions? serializerOptions = null)
        {
            var dictionary = await contentSet.GetAsJson<IDictionary<string, T>>(name, serializerOptions)
                .ConfigureAwait(false);
            KeyValuePair<string, T> matchingEntry = dictionary.FirstOrDefault(kvp => kvp.Key.Equals(key, keyComparison));
            return matchingEntry.Value;
        }
    }
}
