using Moq;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace NetDaemonTest.Apps;

public class NotifyAppTest : TestBase
{
    const string imageName = "imageName";

    private void SetupDefaultMocks()
    {
        HouseNotificationImageCreatorMock.Setup(x => x.AddFormattedText(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Func<string?>>()));
        //HouseNotificationImageCreatorMock.Setup(x => x.AddFormattedText(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<Func<string?>>>()));
        HouseNotificationImageCreatorMock.Setup(x => x.AddConditionalImage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<byte[]>(), It.IsAny<Func<bool>?>()));
        HouseNotificationImageCreatorMock.Setup(x => x.CreateImage());
        HouseNotificationImageCreatorMock.Setup(x => x.GetImagePath()).Returns(imageName);
        NotifyMock.Setup(x => x.NotifyHouseStateGsmKen("House State", "HA startup", imageName, NotifyPriorityEnum.high, It.IsAny<List<NotifyActionEnum>>()));
    }

    [Fact]
    public void NotifyApp_Constructor_CheckEvents()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();

        // Act
        var app = Context.GetApp<NotifyApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_DailyBedAlarm_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        SettingsProviderMock.Setup(x => x.BeddenAlarmKids).Returns(true);
        HaMock.TriggerStateChange(Entities.InputDatetime.Beddenalarmkids, "18:30");
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(18).AddMinutes(29).ToUniversalTime().Ticks);
        NotifyMock.Setup(x => x.NotifyHouse("Attentie, Damon en Caitlyn jullie mogen je bed aan zetten."));

        // Act
        var app = Context.GetApp<NotifyApp>();
        Scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_DailyBedAlarmWithTimeChange_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        SettingsProviderMock.Setup(x => x.BeddenAlarmKids).Returns(true);
        HaMock.TriggerStateChange(Entities.InputDatetime.Beddenalarmkids, "17:30");
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(18).AddMinutes(29).ToUniversalTime().Ticks);
        NotifyMock.Setup(x => x.NotifyHouse("Attentie, Damon en Caitlyn jullie mogen je bed aan zetten."));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.InputDatetime.Beddenalarmkids, "18:30");
        Scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_DailyBedAlarmDisabled_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        SettingsProviderMock.Setup(x => x.BeddenAlarmKids).Returns(false);
        HaMock.TriggerStateChange(Entities.InputDatetime.Beddenalarmkids, "18:30");
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(18).AddMinutes(29).ToUniversalTime().Ticks);

        // Act
        var app = Context.GetApp<NotifyApp>();
        Scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_HouseState_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        HouseNotificationImageCreatorMock.Setup(x => x.CreateImage());
        HouseNotificationImageCreatorMock.Setup(x => x.GetImagePath()).Returns(imageName);

        NotifyMock.Setup(x => x.NotifyHouseStateGsmKen("House State", "House state: Awake", imageName, NotifyPriorityEnum.low,
            It.Is<List<NotifyActionEnum>>(x => x.Count == 3 && x.Contains(NotifyActionEnum.Thermostat17) && x.Contains(NotifyActionEnum.Thermostat20) && x.Contains(NotifyActionEnum.UriThermostat))));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.InputText.Housestate, "Awake");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_DayNightState_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        HouseNotificationImageCreatorMock.Setup(x => x.CreateImage());
        HouseNotificationImageCreatorMock.Setup(x => x.GetImagePath()).Returns(imageName);

        NotifyMock.Setup(x => x.NotifyHouseStateGsmKen("House State", "Day/Night state: Day", imageName, NotifyPriorityEnum.low,
            It.Is<List<NotifyActionEnum>>(x => x.Count == 3 && x.Contains(NotifyActionEnum.Thermostat17) && x.Contains(NotifyActionEnum.Thermostat20) && x.Contains(NotifyActionEnum.UriThermostat))));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Day");

        // Assert
        VerifyAllMocks();
    }


    [Fact]
    public void NotifyApp_ThermostatChange_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        HouseNotificationImageCreatorMock.Setup(x => x.CreateImage());
        HouseNotificationImageCreatorMock.Setup(x => x.GetImagePath()).Returns(imageName);

        NotifyMock.Setup(x => x.NotifyHouseStateGsmKen("House State", "Thermostat: 25", imageName, NotifyPriorityEnum.high,
            It.Is<List<NotifyActionEnum>>(x => x.Count == 3 && x.Contains(NotifyActionEnum.Thermostat17) && x.Contains(NotifyActionEnum.Thermostat20) && x.Contains(NotifyActionEnum.UriThermostat))));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Sensor.TempKeukenSetpoint, "25");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_PowerTarifChange_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        HouseNotificationImageCreatorMock.Setup(x => x.CreateImage());
        HouseNotificationImageCreatorMock.Setup(x => x.GetImagePath()).Returns(imageName);

        NotifyMock.Setup(x => x.NotifyHouseStateGsmKen("House State", "Energy tarif: Test", imageName, NotifyPriorityEnum.low, It.IsAny<List<NotifyActionEnum>>()));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Sensor.PowerTariff, "Test");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_LocationChangeKenWerkAfterTime_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(16).ToUniversalTime().Ticks);
        HaMock.TriggerStateChange(Entities.Zone.WerkKen, "WerkKen", new ZoneAttributes { FriendlyName = "Werk Ken" });
        HaMock.TriggerStateChange(Entities.Person.Ken, Entities.Zone.WerkKen.Attributes?.FriendlyName!);
        //NotifyMock.Setup(x => x.NotifyGsmGreet("Ken lokatie", "Ken is vertrokken vanuit werk", NotifyPriorityEnum.high, null, null));
        //NotifyMock.Setup(x => x.NotifyHouse("Attentie, Ken is vertrokken vanuit werk"));
        NotifyMock.Setup(x => x.NotifyHouseStateGsmKen("House State", "Ken: Away", imageName, NotifyPriorityEnum.low, It.IsAny<List<NotifyActionEnum>>()));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Person.Ken, "Away");


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_LocationChangeKenWerkBeforeTime_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(12).ToUniversalTime().Ticks);
        HaMock.TriggerStateChange(Entities.Zone.WerkKen, "WerkKen", new ZoneAttributes { FriendlyName = "Werk Ken" });
        HaMock.TriggerStateChange(Entities.Person.Ken, Entities.Zone.WerkKen.Attributes?.FriendlyName!);
        NotifyMock.Setup(x => x.NotifyHouseStateGsmKen("House State", "Ken: Away", imageName, NotifyPriorityEnum.low, It.IsAny<List<NotifyActionEnum>>()));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Person.Ken, "Away");


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_LocalCalendarOff_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        var app = Context.GetApp<NotifyApp>();
        // Act
        HaMock.TriggerStateChange(Entities.Calendar.Local, "off");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_LocalCalendarOn_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        var app = Context.GetApp<NotifyApp>();
        NotifyMock.Setup(x => x.NotifyGsmKen("Calendar notify test", "Test", NotifyPriorityEnum.high, null, null));

        // Act
        HaMock.TriggerStateChange(Entities.Calendar.Local, "on", new CalendarAttributes() { FriendlyName = "[NotifyHouse] Test" });

        // Assert
        VerifyAllMocks();
    }


    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void NotifyApp_OpenClose_VerifyCalls(NetDaemon.HassModel.Entities.Entity entity, NotifyTagEnum tag, NotifyActionEnum action)
    {
        // Arrange
        ResetAllMocks();
        SetupDefaultMocks();
        HaMock.TriggerStateChange(entity, "off");
        NotifyMock.Setup(x => x.NotifyGsmKen("", It.IsAny<string>(), NotifyPriorityEnum.high, tag, It.Is<List<NotifyActionEnum>>(y => y.Count == 1 && y.Contains(action))));
        NotifyMock.Setup(x => x.Clear(tag));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(entity, "on");
        Scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);
        HaMock.TriggerStateChange(entity, "off");

        // Assert
        VerifyAllMocks();
    }



    public class TestDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new();

        public TestDataGenerator()
        {
            var entities = new Entities(null!);

            _data.Add(new object[] { entities.BinarySensor.OpencloseVoordeur, NotifyTagEnum.OpenCloseVoordeur, NotifyActionEnum.OpenCloseVoordeurOmroepen });
            _data.Add(new object[] { entities.BinarySensor.OpencloseGarage, NotifyTagEnum.OpenCloseGarage, NotifyActionEnum.OpenCloseGarageOmroepen });
            _data.Add(new object[] { entities.BinarySensor.OpencloseAchterdeur, NotifyTagEnum.OpenCloseAchterdeur, NotifyActionEnum.OpenCloseAchterdeurOmroepen });
            _data.Add(new object[] { entities.BinarySensor.OpencloseTuindeur, NotifyTagEnum.OpenCloseTuindeur, NotifyActionEnum.OpenCloseTuindeurOmroepen });
            _data.Add(new object[] { entities.BinarySensor.OpencloseAchterdeurgarage, NotifyTagEnum.OpenCloseAchterdeurgarage, NotifyActionEnum.OpenCloseAchterdeurgarageOmroepen });
        }

        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}