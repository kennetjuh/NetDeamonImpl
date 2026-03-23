using System.Collections.Generic;

namespace NetDaemonImpl
{
    public class ActionCollection
    {
        private readonly Dictionary<string, Action> Actions = [];

        public void Delete(string entityId)
        {
            Actions.Remove(entityId);
        }

        public void AddOrUpdate(string entityId, Action action)
        {
            Actions[entityId] = action;
        }

        public void ExecuteActions()
        {
            foreach (var action in Actions)
            {
                action.Value.Invoke();
            }
        }
    }
}
