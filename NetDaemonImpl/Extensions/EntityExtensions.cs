using System.Text.Json.Serialization;

namespace NetDaemonImpl.Extensions
{
    public static class EntityExtensions
    {
        public static bool IsOn(this LightEntity x)
        {
            return x.State == "on";
        }

        public static bool IsOff(this LightEntity x)
        {
            return x.State == "off";
        }

        public static bool IsOn(this SwitchEntity x)
        {
            return x.State == "on";
        }

        public static bool IsOff(this SwitchEntity x)
        {
            return x.State == "off";
        }

        public static bool IsOn(this InputBooleanEntity x)
        {
            return x.State == "on";
        }

        public static bool IsOff(this InputBooleanEntity x)
        {
            return x.State == "off";
        }

        public static void SetDatetime(this InputDatetimeEntity target, string? date = null, string? time = null, string? datetime = null, long? timestamp = null)
        {
            target.CallService("set_datetime", new MyInputDatetimeSetDatetimeParameters { Date = date, Time = time, Datetime = datetime, Timestamp = timestamp });
        }
    }

    public partial record MyInputDatetimeSetDatetimeParameters
    {
        ///<summary>The target date. eg: &quot;2019-04-20&quot;</summary>
        [JsonPropertyName("date")]
        public string? Date { get; init; }

        ///<summary>The target time. eg: &quot;05:04:20&quot;</summary>
        [JsonPropertyName("time")]
        public string? Time { get; init; }

        ///<summary>The target date &amp; time. eg: &quot;2019-04-20 05:04:20&quot;</summary>
        [JsonPropertyName("datetime")]
        public string? Datetime { get; init; }

        ///<summary>The target date &amp; time, expressed by a UNIX timestamp.</summary>
        [JsonPropertyName("timestamp")]
        public long? Timestamp { get; init; }
    }
}
