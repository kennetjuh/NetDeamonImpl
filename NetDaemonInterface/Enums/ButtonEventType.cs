using System;
using System.Collections.Generic;

namespace NetDaemonInterface.Enums;

public enum ButtonEventType
{
    Unknown = 0,
    down_close =1,
    up_open = 2,
    Single = 1002,
    Release = 1003,
    Double = 1004,
    LongPress = 1001,
    
    //stop
}


public static class ButtonEventTypeExtensions
{
    private static readonly Dictionary<string, ButtonEventType> _fromRaw =
        new(StringComparer.OrdinalIgnoreCase)
        {
                    { "single", ButtonEventType.Single},
                    { "double", ButtonEventType.Double},
                    { "hold", ButtonEventType.LongPress},                    
                    { "down_close", ButtonEventType.down_close},
                    { "up_open", ButtonEventType.up_open},
        };

    // Reverse map for ToRaw - keep in sync with _fromRaw
    private static readonly Dictionary<ButtonEventType, string> _toRaw =
        BuildReverseMap();

    private static Dictionary<ButtonEventType, string> BuildReverseMap()
    {
        var d = new Dictionary<ButtonEventType, string>();
        foreach (var kv in _fromRaw)
            d[kv.Value] = kv.Key;
        d[ButtonEventType.Unknown] = "unknown";
        return d;
    }

    // Extension: parse a raw string to enum; returns Unknown for unmapped values
    public static ButtonEventType ToButtonEventType(this string? raw)
    {
        if (raw is null) return ButtonEventType.Unknown;
        return _fromRaw.TryGetValue(raw, out var v) ? v : ButtonEventType.Unknown;
    }

    // Extension: get canonical raw string for an enum value
    public static string ToRaw(this ButtonEventType id)
    {
        return _toRaw.TryGetValue(id, out var s) ? s : "unknown";
    }

    // Extension: allow checking if a raw id is known
    public static bool IsKnown(this string? raw) => raw is not null && _fromRaw.ContainsKey(raw);
}