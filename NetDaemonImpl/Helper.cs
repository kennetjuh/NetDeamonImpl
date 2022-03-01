using NetDaemonInterface;
using System.Globalization;

namespace NetDaemonImpl
{
    public static class Helper
    {
        public static DateTime StringToDateTime(string? time)
        {
            if (time == null)
            {
                return DateTime.MinValue;
            }
            return DateTime.ParseExact(time, Constants.dateTime_TimeFormat, CultureInfo.InvariantCulture);
        }

        public static DayNightEnum GetDayNightState(IEntities entities)
        {
            if (Enum.TryParse(typeof(DayNightEnum), entities.InputText.Daynight.State, out var result))
            {
                if (result == null)
                {
                    return DayNightEnum.Day;
                }
                return (DayNightEnum)result;
            }
            return DayNightEnum.Day;
        }

        public static HouseStateEnum GetHouseState(IEntities entities)
        {
            if (Enum.TryParse(typeof(HouseStateEnum), entities.InputText.Housestate.State, out var result))
            {
                if (result == null)
                {
                    return HouseStateEnum.Awake;
                }
                return (HouseStateEnum)result;
            }
            return HouseStateEnum.Awake;
        }
    }
}
