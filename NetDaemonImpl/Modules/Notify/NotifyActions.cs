using Microsoft.Extensions.Options;
using NetDaemonInterface;
using NetDaemonInterface.Models;
using System.Collections.Generic;
using System.Linq;

namespace NetDaemonImpl.Modules.Notify
{
    public class NotifyActions
    {
        private readonly Entities Entities;
        private readonly Services Services;
        public Dictionary<NotifyActionEnum, NotifyAction> Actions { get; set; }

        public NotifyActions(IHaContext haContext, INotify notify, IThinginoClient thinginoClient, IOptions<ThinginoSettings> options)
        {
            Entities = new Entities(haContext);
            Services = new Services(haContext);
            var settings = options?.Value ?? new ThinginoSettings();

            var actions = new List<NotifyAction>()
            {
                new NotifyAction(NotifyActionEnum.Thermostat15, "Set to 15", ()=>Entities.Climate.TadoSmartThermostatRu3248113920.SetTemperature(15)),
                new NotifyAction(NotifyActionEnum.Thermostat19, "Set to 19", ()=>Entities.Climate.TadoSmartThermostatRu3248113920.SetTemperature(19)),
                new NotifyAction(NotifyActionEnum.UriThermostat, "More", $"entityId:{Entities.Climate.TadoSmartThermostatRu3248113920.EntityId}"),
                new NotifyAction(NotifyActionEnum.StopRinger, "Stop Ringer", ()=>thinginoClient.StopDeurbel(settings?.Voordeur!)),
                new NotifyAction(NotifyActionEnum.UriDeurbel, "Open Camera", $"https://frigate.kennetjuh.duckdns.org/#voordeur"),
            };
            Actions = actions.ToDictionary(x => x.Id, x => x);
        }
    }
}
