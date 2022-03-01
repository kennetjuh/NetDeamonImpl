namespace NetDaemonInterface
{
    public interface IHouseState
    {
        void HouseStateAwake();
        void HouseStateAway(HouseStateEnum state);
        void HouseStateSleeping();
        void HouseStateHoliday();
        void TvMode();
    }
}