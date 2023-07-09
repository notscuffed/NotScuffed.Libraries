using System;
using System.Collections.Generic;
using System.Linq;

#if NET5_0_OR_GREATER

namespace NotScuffed.Common;

public static class CommandLine
{
    private static Dictionary<string, string> _options =
        new Dictionary<string, string>();

    private static HashSet<string> _flags =
        new HashSet<string>();

    private static List<string> _args =
        new List<string>();

    public static IReadOnlyDictionary<string, string> Options => _options;
    public static IReadOnlySet<string> Flags => _flags;
    public static IReadOnlyList<string> Args => _args;

    public static void Init(string[] args, bool ignoreCase = true)
    {
        var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

        _options = args
            .Where(x => x[0] == '-' && x.IndexOf('=') > 0)
            .ToDictionary(
                x => x[(x[1] == '-' ? 2 : 1)..x.IndexOf('=')],
                x => x[(x.IndexOf('=') + 1)..],
                comparer);

        _flags = args
            .Where(x => x[0] == '-' && !x.Contains('='))
            .Select(x => x[(x[1] == '-' ? 2 : 1)..])
            .ToHashSet(comparer);

        _args = args.Where(x => x[0] != '-').ToList();
    }
}

#endif