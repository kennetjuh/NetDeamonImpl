using System;
using System.Collections.Generic;

namespace NetDaemonInterface.Enums
{
    public enum Button
    {
        Unknown,
        HouseInkom,
        HouseVoordeur,
        Keuken1,
        Keuken2,
        Keuken3,
        Bank,
        Woonkamer,
        Gordijn,
        Eetkamer,
        SlaapkamerCaitlyn1,
        SlaapkamerCaitlyn2,
        SlaapkamerCaitlyn3,
        Badkamer,
        Inkom1,
        Washal,
        SlaapkamerKen,
        SlaapkamerKenBed,
        SlaapkamerDamon,
        Rommelkamer,
        HalBoven1,
        HalBoven2,
        HalBoven3,
        Garage,
        Kip,
    }

    public static class ButtonIdExtensions
    {
        private static readonly Dictionary<string, Button> _fromRaw =
            new(StringComparer.OrdinalIgnoreCase)
            {
                    { "2f9f760cf1350ffd4614d1947115edea", Button.HouseInkom },
                    { "064f4bfe12d6d7743cde8b427f53cb5a", Button.HouseVoordeur },
                    { "53c54965a6deceeac6add3212dc7246a", Button.Keuken1 },
                    { "9a011d7c2a67a7d173a80ee82533c8ac", Button.Keuken2 },
                    { "ded92b5678b3aad92a87c898105266c3", Button.Keuken3 },
                    { "5a6c7fad830336920fe922bf816f2256", Button.Bank },
                    { "ebf17b3eefc51e17d5d363a944c822b5", Button.Woonkamer },
                    { "4646d26e9a4a201390f6f9a16cd984e0", Button.Gordijn },
                    { "144aef20285bfef0128680117648dc0d", Button.Eetkamer },
                    { "5da2d9e771f9736c0c877dd15394a4ea", Button.SlaapkamerCaitlyn1 },
                    { "83f202a5686987a3d016b8dec996a679", Button.SlaapkamerCaitlyn2 },
                    { "e258df90a35081d56f3ff713d19c3979", Button.SlaapkamerCaitlyn3 },
                    { "fe2db6a1aec5f5dc65777df0346b2bbf", Button.Badkamer },
                    { "c419133291473399726aac4faa63c8b2", Button.Inkom1 },
                    { "61c3e611f13428f09386f453b74795f8", Button.Washal },
                    { "baf3c4bd350ff84c35a2ab80edeef5d9", Button.SlaapkamerKen },
                    { "1a87ff8d06301bb8bc87c91884f84d7c", Button.SlaapkamerKenBed },
                    { "01848084ee3e4522301898b2ea7623d6", Button.SlaapkamerDamon },
                    { "fa7fd40704832d813275f2c256856954", Button.Rommelkamer },
                    { "720515d679d9621ae6efafdc2e2fe8a5", Button.HalBoven1 },
                    { "1ed39a673d45097bb7b3541d2629383a", Button.HalBoven2 },
                    { "74ff245e2723525a9b6f6a6dea0fe811", Button.HalBoven3 },
                    { "959b1338c5d0619bce3c0164e31184fd", Button.Garage },
                    { "becbdc29ed9e8e39d220feed7e215e7a", Button.Kip },
            };

        // Reverse map for ToRaw - keep in sync with _fromRaw
        private static readonly Dictionary<Button, string> _toRaw =
            BuildReverseMap();

        private static Dictionary<Button, string> BuildReverseMap()
        {
            var d = new Dictionary<Button, string>();
            foreach (var kv in _fromRaw)
                d[kv.Value] = kv.Key;
            d[Button.Unknown] = "unknown";
            return d;
        }

        // Extension: parse a raw string to enum; returns Unknown for unmapped values
        public static Button ToButtonId(this string? raw)
        {
            if (raw is null) return Button.Unknown;
            return _fromRaw.TryGetValue(raw, out var v) ? v : Button.Unknown;
        }

        // Extension: get canonical raw string for an enum value
        public static string ToRaw(this Button id)
        {
            return _toRaw.TryGetValue(id, out var s) ? s : "unknown";
        }

        // Extension: allow checking if a raw id is known
        public static bool IsKnown(this string? raw) => raw is not null && _fromRaw.ContainsKey(raw);
    }
}
