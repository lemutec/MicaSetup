using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MicaSetup.Helper;

public static class JsonHelper
{
    public static string SerializeObject(IDictionary<string, dynamic?> obj)
    {
        StringBuilder json = new("{");
        bool first = true;

        foreach (var kvp in obj)
        {
            if (!first)
            {
                json.Append(",");
            }
            first = false;

            json.Append($"\"{Regex.Escape(kvp.Key)}\":");
            if (kvp.Value == null)
            {
                json.Append("null");
            }
            else if (kvp.Value is string)
            {
                json.Append($"\"{Regex.Escape(kvp.Value.ToString()!)}\"");
            }
            else if (kvp.Value is double)
            {
                json.Append(kvp.Value.ToString());
            }
            else if (kvp.Value is bool)
            {
                json.Append(kvp.Value.ToString().ToLower());
            }
        }

        json.Append("}");
        return json.ToString();
    }

    public static IDictionary<string, dynamic?> DeserializeObject(string json)
    {
        Dictionary<string, dynamic?> obj = [];
        Regex regex = new("\"(?<key>[^\"]+)\"\\s*:\\s*(?<value>null|\"(?<string>[^\"]*)\"|(?<number>\\d+(?:\\.\\d+)?|true|false))");
        MatchCollection matches = regex.Matches(json);

        foreach (Match match in matches)
        {
            string key = match.Groups["key"].Value;

            if (match.Groups["value"].Value == "null")
            {
                obj[key] = null;
            }
            else if (match.Groups["string"].Success)
            {
                obj[key] = match.Groups["string"].Value;
            }
            else if (match.Groups["number"].Success)
            {
                if (double.TryParse(match.Groups["number"].Value, out double number))
                {
                    obj[key] = number;
                }
                else if (bool.TryParse(match.Groups["number"].Value, out bool boolean))
                {
                    obj[key] = boolean;
                }
            }
        }

        return obj;
    }
}
