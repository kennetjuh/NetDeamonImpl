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
    [Fact]
    public void NotifyApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

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
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(17).AddMinutes(59).ToUniversalTime().Ticks);
        NotifyMock.Setup(x => x.NotifyHouse("Attentie, Damon en Caitlyn jullie mogen je bed aan zetten."));

        // Act
        var app = Context.GetApp<NotifyApp>();
        Scheduler.AdvanceBy(TimeSpan.FromMinutes(1).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_ThermostatChange_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        NotifyMock.Setup(x => x.NotifyGsmKen("", "Thermostat changed to 25", NotifyTagEnum.ThermostatChanged,
            It.Is<List<NotifyActionEnum>>(x => x.Count == 3 && x.Contains(NotifyActionEnum.Thermostat15) && x.Contains(NotifyActionEnum.Thermostat21) && x.Contains(NotifyActionEnum.UriThermostat))));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Sensor.TempKeukenSetpoint, "25");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_LocationChangeKenWerkAfterTime_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(15).ToUniversalTime().Ticks);
        HaMock.TriggerStateChange(Entities.Zone.WerkKen, "WerkKen", new ZoneAttributes {FriendlyName = "Werk Ken" });
        HaMock.TriggerStateChange(Entities.Person.Ken, Entities.Zone.WerkKen.Attributes?.FriendlyName!);
        NotifyMock.Setup(x=>x.NotifyGsmGreet("Ken lokatie", "Ken is vertrokken vanuit werk",null,null));
        NotifyMock.Setup(x=>x.NotifyHouse("Attentie, Ken is vertrokken vanuit werk"));

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
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(12).ToUniversalTime().Ticks);
        HaMock.TriggerStateChange(Entities.Zone.WerkKen, "WerkKen", new ZoneAttributes {FriendlyName = "Werk Ken" });
        HaMock.TriggerStateChange(Entities.Person.Ken, Entities.Zone.WerkKen.Attributes?.FriendlyName!);

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Person.Ken, "Away");


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_LocationChangeGreetWerkAfterTime_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(15).ToUniversalTime().Ticks);
        HaMock.TriggerStateChange(Entities.Zone.WerkGreet, "WerkGreet", new ZoneAttributes {FriendlyName = "Werk Greet" });
        HaMock.TriggerStateChange(Entities.Person.Greet, Entities.Zone.WerkGreet.Attributes?.FriendlyName!);
        NotifyMock.Setup(x => x.NotifyGsmKen("Greet lokatie", "Greet is vertrokken vanuit werk", null, null));
        NotifyMock.Setup(x => x.NotifyHouse("Attentie, Great is vertrokken vanuit werk"));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Person.Greet, "Away");


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_LocationChangeGreetWerkBeforeTime_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(12).ToUniversalTime().Ticks);
        HaMock.TriggerStateChange(Entities.Zone.WerkGreet, "WerkGreet", new ZoneAttributes {FriendlyName = "Werk Greet" });
        HaMock.TriggerStateChange(Entities.Person.Greet, Entities.Zone.WerkGreet.Attributes?.FriendlyName!);

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Person.Greet, "Away");


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void NotifyApp_LocationChangeGreetIjzerenMan_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Zone.IjzerenMan, "IjzerenMan", new ZoneAttributes { FriendlyName = "Ijzeren Man" });
        HaMock.TriggerStateChange(Entities.Person.Greet, Entities.Zone.IjzerenMan.Attributes?.FriendlyName!);
        NotifyMock.Setup(x => x.NotifyGsmKen("Greet lokatie", "Greet is vertrokken vanuit de ijzeren man", null, null));
        NotifyMock.Setup(x => x.NotifyHouse("Attentie, Great is vertrokken vanuit de ijzeren man"));

        // Act
        var app = Context.GetApp<NotifyApp>();
        HaMock.TriggerStateChange(Entities.Person.Greet, "Away");


        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [ClassData(typeof(TestDataGenerator))]
    public void NotifyApp_OpenClose_VerifyCalls(NetDaemon.HassModel.Entities.Entity entity, NotifyTagEnum tag, NotifyActionEnum action)
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(entity, "off");
        NotifyMock.Setup(x => x.NotifyGsmKen("", It.IsAny<string>(), tag, It.Is<List<NotifyActionEnum>>(y => y.Count == 1 && y.Contains(action))));
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