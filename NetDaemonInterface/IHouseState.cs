namespace NetDaemonInterface
{
    public interface IHouseState
    {
        void HouseStateAwake();
        void HouseStateAway();
        void HouseStateSleeping();
    }
}