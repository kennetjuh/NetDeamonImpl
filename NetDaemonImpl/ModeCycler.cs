namespace NetDaemonImpl
{
    public class ModeCycler
    {
        private readonly TimeSpan timeout;
        private readonly Action[] Actions;
        private int mode = -1;
        private DateTime lastCycleTime = DateTime.Now;

        public ModeCycler(TimeSpan timeout, params Action[] actions)
        {
            this.timeout = timeout;
            Actions = actions;
        }

        public void Cycle()
        {
            if (DateTime.Now - lastCycleTime > timeout)
            {
                mode = -1;
            }
            mode++;
            if (mode > Actions.Length - 1)
            {
                mode = 0;
            }
            Actions[mode]();
            lastCycleTime = DateTime.Now;
        }
    }
}
