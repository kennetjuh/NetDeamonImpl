using Moq;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using System.Collections.Generic;
using Xunit;

namespace NetDaemonTest.Apps;

public class NotifyAppTest : TestBase
{
    const string imageName = "imageName";

    private void SetupDefaultMocks()
    {
        HouseNotificationImageCreatorMock.Setup(x => x.AddFormattedText(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<Func<string?>>()));
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
            It.Is<List<NotifyActionEnum>>(x => x.Count == 3 && x.Contains(NotifyActionEnum.Thermostat15) && x.Contains(NotifyActionEnum.Thermostat19) && x.Contains(NotifyActionEnum.UriThermostat))));

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
            It.Is<List<NotifyActionEnum>>(x => x.Count == 3 && x.Contains(NotifyActionEnum.Thermostat15) && x.Contains(NotifyActionEnum.Thermostat19) && x.Contains(NotifyActionEnum.UriThermostat))));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.InputText.Daynight, "Day");

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
}