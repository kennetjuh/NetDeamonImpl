using NetDaemonInterface;
using System.Collections.Generic;
using System.Linq;

namespace NetDaemonImpl.Modules.Notify
{
    public class NotifyActions
    {
        private readonly Entities Entities;
        private readonly Services Services;
        public Dictionary<NotifyActionEnum, NotifyAction> Actions { get; set; }

        public NotifyActions(IHaContext haContext, INotify notify)
        {
            Entities = new Entities(haContext);
            Services = new Services(haContext);

            var actions = new List<NotifyAction>()
            {
                new NotifyAction(NotifyActionEnum.Thermostat17, "Set to 17", ()=>Entities.Climate.Keuken.SetTemperature(17)),
                new NotifyAction(NotifyActionEnum.Thermostat20, "Set to 20", ()=>Entities.Climate.Keuken.SetTemperature(20)),
                new NotifyAction(NotifyActionEnum.UriThermostat, "More", $"entityId:{Entities.Climate.Keuken.EntityId}"),
                new NotifyAction(NotifyActionEnum.OpenCloseVoordeurOmroepen, "Omroepen", ()=>notify.NotifyHouse("De voordeur staat al lange tijd open")),
                new NotifyAction(NotifyActionEnum.OpenCloseAchterdeurgarageOmroepen, "Omroepen", ()=>notify.NotifyHouse("De achterdeur in de garage staat al lange tijd open")),
                new NotifyAction(NotifyActionEnum.OpenCloseGarageOmroepen, "Omroepen", ()=>notify.NotifyHouse("De garage staat al lange tijd open")),
                new NotifyAction(NotifyActionEnum.OpenCloseTuindeurOmroepen, "Omroepen", ()=>notify.NotifyHouse("De tuindeur staat al lange tijd open")),
                new NotifyAction(NotifyActionEnum.OpenCloseAchterdeurOmroepen, "Omroepen", ()=>notify.NotifyHouse("De achterdeur staat al lange tijd open")),
            };
            Actions = actions.ToDictionary(x => x.Id, x => x);
        }
    }
}
