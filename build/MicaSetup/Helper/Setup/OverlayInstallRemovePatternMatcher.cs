using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MicaSetup.Helper;

/// <summary>
/// Matches relative install-path globs for overlay cleanup (gitignore-like).
/// Positive patterns mark files for deletion; patterns prefixed with "!" keep matches.
/// </summary>
public sealed class OverlayInstallRemovePatternMatcher
{
    private readonly List<Regex> _positive = [];
    private readonly List<Regex> _negative = [];

    public OverlayInstallRemovePatternMatcher(IEnumerable<string> patterns)
    {
        foreach (string raw in patterns)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                continue;
            }

            string pattern = raw.Trim();

            // gitignore: lines starting with '#' are comments
            if (pattern.StartsWith("#", StringComparison.Ordinal))
            {
                continue;
            }

            // gitignore: '!' negates; '\!' means a literal '!' at the start of the pattern
            bool keep = false;
            if (pattern.StartsWith("\\!", StringComparison.Ordinal))
            {
                pattern = pattern.Substring(1);
            }
            else if (pattern.StartsWith("!", StringComparison.Ordinal))
            {
                keep = true;
                pattern = pattern.Substring(1).Trim();
            }

            if (string.IsNullOrEmpty(pattern))
            {
                continue;
            }

            Regex regex = GlobToRegex(pattern);
            if (keep)
            {
                _negative.Add(regex);
            }
            else
            {
                _positive.Add(regex);
            }
        }
    }

    public bool IsEmpty => _positive.Count == 0 && _negative.Count == 0;

    public bool HasKeepPatterns => _negative.Count > 0;

    /// <summary>
    /// Returns true when <paramref name="relativePath"/> matches a gitignore-style "!" keep pattern.
    /// Used by Ext cleanup so keep patterns can veto extension-based deletes.
    /// </summary>
    public bool IsKept(string relativePath)
    {
        if (_negative.Count == 0)
        {
            return false;
        }

        string normalized = NormalizeRelativePath(relativePath);
        if (string.IsNullOrEmpty(normalized))
        {
            return false;
        }

        foreach (Regex regex in _negative)
        {
            if (regex.IsMatch(normalized))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns true when <paramref name="relativePath"/> matches at least one positive
    /// pattern and no negative (!) pattern.
    /// </summary>
    public bool ShouldRemove(string relativePath)
    {
        if (_positive.Count == 0)
        {
            return false;
        }

        string normalized = NormalizeRelativePath(relativePath);
        if (string.IsNullOrEmpty(normalized))
        {
            return false;
        }

        bool matched = false;
        foreach (Regex regex in _positive)
        {
            if (regex.IsMatch(normalized))
            {
                matched = true;
                break;
            }
        }

        if (!matched)
        {
            return false;
        }

        return !IsKept(normalized);
    }

    public static string GetRelativePath(string baseDirectory, string fullPath)
    {
        string baseFull = Path.GetFullPath(baseDirectory)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string fileFull = Path.GetFullPath(fullPath);

        if (!fileFull.StartsWith(baseFull, StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        if (fileFull.Length == baseFull.Length)
        {
            return string.Empty;
        }

        int start = baseFull.Length;
        if (fileFull[start] == Path.DirectorySeparatorChar || fileFull[start] == Path.AltDirectorySeparatorChar)
        {
            start++;
        }

        return fileFull.Substring(start);
    }

    private static string NormalizeRelativePath(string relativePath)
    {
        return relativePath
            .Replace('\\', '/')
            .TrimStart('/');
    }

    private static Regex GlobToRegex(string glob)
    {
        string normalized = NormalizeRelativePath(glob);

        if (normalized.IndexOf('/') < 0)
        {
            normalized = "**/" + normalized;
        }

        StringBuilder sb = new();
        sb.Append('^');

        for (int i = 0; i < normalized.Length; i++)
        {
            char c = normalized[i];

            if (c == '*' && i + 1 < normalized.Length && normalized[i + 1] == '*')
            {
                // ** optionally followed by /
                if (i + 2 < normalized.Length && normalized[i + 2] == '/')
                {
                    sb.Append("(?:.*/)?");
                    i += 2;
                }
                else
                {
                    sb.Append(".*");
                    i++;
                }

                continue;
            }

            switch (c)
            {
                case '*':
                    sb.Append("[^/]*");
                    break;
                case '?':
                    sb.Append("[^/]");
                    break;
                case '/':
                    sb.Append('/');
                    break;
                default:
                    sb.Append(Regex.Escape(c.ToString()));
                    break;
            }
        }

        sb.Append('$');
        return new Regex(sb.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }
}
