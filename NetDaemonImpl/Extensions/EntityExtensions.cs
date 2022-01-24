using NetDaemon.HassModel.Entities;

namespace NetDaemonImpl.Extensions
{
    public static class EntityExtensions
    {
        public static void SetState(this Entity x, IServices services, string state)
        {
            services.Netdaemon.EntityUpdate(x.EntityId, state: state);
        }

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
    }
}
