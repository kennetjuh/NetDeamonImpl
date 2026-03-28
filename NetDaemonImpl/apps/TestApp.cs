using Microsoft.Extensions.Hosting;
using NetDaemon.Client;
using NetDaemonInterface;
using NetDaemonInterface.Observable;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
//[Focus]
public class TestApp : MyNetDaemonBaseApp
{
    public TestApp(IHomeAssistantRunner runner, ITriggerManager triggerManager, IHaContext haContext, IScheduler scheduler, ILogger<TestApp> logger, IDelayProvider delayProvider, ILightControl lightControl,
        ILuxBasedBrightness luxBasedBrightness, INotify notify, ISettingsProvider settingsProvider, IHouseNotificationImageCreator houseNotificationImageCreator, IButtonEvents deconzButtonEvents, IThinginoClient thinginoClient, IFrigateClient frigateClient)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        //thinginoClient.Connect("192.168.1.50");
        //thinginoClient.Deurbel("192.168.1.50");
        //Task.Run(async () =>
        //{
            
        //            await Task.Delay(5000);
        //    thinginoClient.StopDeurbel("192.168.1.50");


        //});
        //thinginoClient.StopDeurbel("192.168.1.50");

        //var test = _entities.Climate.TadoSmartThermostatRu3248113920.EntityState;
        //logger.LogInformation(_entities.Climate.TadoSmartThermostatRu3248113920.EntityState?.Attributes?.Temperature?.ToString("F1"));


        //thinginoClient.SetLight(ipCamera, false);

        //thinginoClient.SetPrivacyMode("192.168.1.50", true);
        //thinginoClient.SetPrivacyMode("192.168.1.50", false);

        //var haMessages = (IHomeAssistantHassMessages)runner.CurrentConnection!;

        //haMessages.OnHassMessage.Subscribe(m => logger.LogInformation("{Message}", m));

        //notify.NotifyHouse("Dit is een test");

        //find al frigate cameras and look for the recording toggle entity        

        //notify.NotifyGsmAlarm();
        //houseNotificationImageCreator.AddFormattedText(5, 10, 10, "Ken: {0}", () => _entities.Person.Ken.State?.ToString());
        //houseNotificationImageCreator.AddFormattedText(5, 20, 10, "Greet: {0}", () => _entities.Person.Greet.State?.ToString());

        //houseNotificationImageCreator.AddConditionalImage(125, 30, 20, 20, Resource.EnergyLow, () => _entities.Sensor.PowerTariff.State?.ToString() == "low");
        //houseNotificationImageCreator.AddConditionalImage(125, 30, 20, 20, Resource.EnergyHigh, () => _entities.Sensor.PowerTariff.State?.ToString() == "normal");

        //houseNotificationImageCreator.AddConditionalImage(150, 50, 50, 50, Resource.Home, null);
        //houseNotificationImageCreator.AddConditionalImage(150, 30, 20, 20, Resource.Moon, ()=>Helper.GetDayNightState(_entities) == DayNightEnum.Night);
        //houseNotificationImageCreator.AddConditionalImage(150, 30, 20, 20, Resource.Sun, ()=>Helper.GetDayNightState(_entities) == DayNightEnum.Day);
        //houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Holiday, ()=>Helper.GetHouseState(_entities) == HouseStateEnum.Holiday);
        //houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Awake, ()=>Helper.GetHouseState(_entities) == HouseStateEnum.Awake);
        //houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Away, ()=>Helper.GetHouseState(_entities) == HouseStateEnum.Away);
        //houseNotificationImageCreator.AddConditionalImage(175, 30, 20, 20, Resource.Sleep, ()=>Helper.GetHouseState(_entities) == HouseStateEnum.Sleeping);

        //houseNotificationImageCreator.AddFormattedText(110, 90, 15, "{0}", () => _entities.Sensor.TempKeukenSetpoint.State?.ToString());
        //houseNotificationImageCreator.AddConditionalImage(125, 70, 25, 25, Resource.Thermostat, null);

        //houseNotificationImageCreator.CreateImage();
        //

        //_haContext.Events            
        //    .Subscribe(x =>
        //    {
        //        if (x.DataElement.ToString()!.ToLower().Contains("calendar"))
        //        {
        //            Console.WriteLine($"{x.EventType} {x.DataElement}");
        //            Console.WriteLine();
        //        }

        //    });
        //var foo = settingsProvider.JimmieAlarm;

        //_entities.InputDatetime.Daynightlastnighttrigger.SetDatetime(time: DateTime.Now.ToString(Constants.dateTime_TimeFormat));
        //_entities.InputDatetime.Daynightlastdaytrigger.SetDatetime(time: DateTime.Now.ToString(Constants.dateTime_TimeFormat));
    }

}
