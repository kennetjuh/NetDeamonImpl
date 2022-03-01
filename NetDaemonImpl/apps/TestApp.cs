using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
//[Focus]
public class TestApp : MyNetDaemonBaseApp
{
    public TestApp(IHaContext haContext, IScheduler scheduler, ILogger<TestApp> logger,
        IAreaCollection areaCollection, IDelayProvider delayProvider, IHouseState houseState, ILightControl lightControl,
        ILuxBasedBrightness luxBasedBrightness, INotify notify, ITwinkle twinkle, ISettingsProvider settingsProvider, IHouseNotificationImageCreator houseNotificationImageCreator)
        : base(haContext, scheduler, logger, settingsProvider)
    {
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


