using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace MicaSetup.Helper;

public static class CommandLineHelper
{
    public static StringDictionary Values { get; private set; } = [];

    /// <summary>
    /// Public properties passed as bare `NAME=value` tokens (MSI style)
    /// </summary>
    public static Dictionary<string, string> Properties { get; } = new(StringComparer.OrdinalIgnoreCase);

    static CommandLineHelper()
    {
        string[] args = Environment.GetCommandLineArgs();
        Regex spliter = new(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex remover = new(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        Regex switcher = new(@"^(-{1,2}|/)", RegexOptions.Compiled);
        Regex property = new(@"^(?<key>[A-Za-z_][\w.]*)=(?<value>.*)$", RegexOptions.Compiled);

        string param = null!;
        string[] parts;

        foreach (string txt in args.Skip(1))
        {
            if (!switcher.IsMatch(txt))
            {
                Match match = property.Match(txt);

                if (match.Success)
                {
                    if (param != null && !Values.ContainsKey(param))
                    {
                        Values.Add(param, bool.TrueString);
                    }
                    param = null!;
                    Properties[match.Groups["key"].Value] = remover.Replace(match.Groups["value"].Value, "$1");
                }
                else if (param != null)
                {
                    if (!Values.ContainsKey(param))
                    {
                        Values.Add(param, remover.Replace(txt, "$1"));
                    }
                    param = null!;
                }
                continue;
            }

            parts = spliter.Split(txt, 3);

            switch (parts.Length)
            {
                case 1:
                    if (param != null)
                    {
                        if (!Values.ContainsKey(param))
                        {
                            parts[0] = remover.Replace(parts[0], "$1");

                            Values.Add(param, parts[0]);
                        }
                        param = null!;
                    }
                    break;

                case 2:
                    if (param != null)
                    {
                        if (!Values.ContainsKey(param))
                        {
                            Values.Add(param, "true");
                        }
                    }
                    param = parts[1];
                    break;

                case 3:
                    if (param != null)
                    {
                        if (!Values.ContainsKey(param))
                        {
                            Values.Add(param, "true");
                        }
                    }

                    param = parts[1];
                    if (!Values.ContainsKey(param))
                    {
                        parts[2] = remover.Replace(parts[2], "$1");
                        Values.Add(param, parts[2]);
                    }

                    param = null!;
                    break;
            }
        }
        if (param != null)
        {
            if (!Values.ContainsKey(param))
            {
                Values.Add(param, bool.TrueString);
            }
        }
    }

    public static bool Has(string key) => Values.ContainsKey(key);

    public static bool? GetValueBoolean(string key)
    {
        bool? ret = null;

        try
        {
            string value = Values[key];

            if (!string.IsNullOrEmpty(value))
            {
                ret = Convert.ToBoolean(value);
            }
        }
        catch
        {
        }
        return ret;
    }

    public static int? GetValueInt32(string key)
    {
        int? ret = null;

        try
        {
            string value = Values[key];

            if (!string.IsNullOrEmpty(value))
            {
                ret = Convert.ToInt32(value);
            }
        }
        catch
        {
        }
        return ret;
    }

    public static double? GetValueDouble(string key)
    {
        double? ret = null;

        try
        {
            string value = Values[key];

            if (!string.IsNullOrEmpty(value))
            {
                ret = Convert.ToDouble(value);
            }
        }
        catch
        {
        }
        return ret;
    }

    public static bool IsValueBoolean(string key)
    {
        return GetValueBoolean(key) ?? false;
    }
}
