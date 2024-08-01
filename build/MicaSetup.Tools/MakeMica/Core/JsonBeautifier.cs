using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MakeMica.Core;

internal static class JsonBeautifier
{
    public static string Beautify(string input)
    {
        object? obj = JsonConvert.DeserializeObject(SortJson(input));

        if (obj != null)
        {
            using StringWriter textWriter = new();
            using JsonTextWriter jsonWriter = new(textWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' ',
            };
            new JsonSerializer().Serialize(jsonWriter, obj);
            return textWriter.ToString();
        }
        else
        {
            return input;
        }
    }

    public static string SortJson(string json)
    {
        SortedDictionary<string, object>? dic = JsonConvert.DeserializeObject<SortedDictionary<string, object>>(json);
        SortedDictionary<string, object> keyValues = new(dic);
        keyValues.OrderBy(m => m.Key);

        SortedDictionary<string, object> tempKeyValues = new(keyValues);
        foreach (KeyValuePair<string, object> kv in tempKeyValues)
        {
            Type t0 = typeof(JObject);
            Type? t1 = kv.Value?.GetType();

            if (t0 == t1)
            {
                string jsonItem = JsonConvert.SerializeObject(kv.Value);
                jsonItem = SortJson(jsonItem);
                keyValues[kv.Key] = JsonConvert.DeserializeObject<JObject>(jsonItem)!;
            }
        }
        return JsonConvert.SerializeObject(keyValues);
    }
}
