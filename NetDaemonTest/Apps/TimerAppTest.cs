using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using System.Linq;
using System.Text.Json;
using Xunit;

namespace NetDaemonTest.Apps;

public class TimerAppTest : TestBase
{
    private const string sleeptimerbeddenFinishedEvent = @"
{
    ""event_type"": ""timer.finished"",
    ""data"": {
        ""entity_id"": ""timer.sleeptimerbedden""      
    } 
}";
    private const string sleeptimerbeddenkidsFinishedEvent = @"
{
    ""event_type"": ""timer.finished"",
    ""data"": {
        ""entity_id"": ""timer.sleeptimerbeddenkids""      
    } 
}";
    private const string sleeptimerkidsFinishedEvent = @"
{
    ""event_type"": ""timer.finished"",
    ""data"": {
        ""entity_id"": ""timer.sleeptimerkids""      
    } 
}";

    [Fact]
    public void TimerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<TimerApp>();

        // Assert
        VerifyAllMocks();
    }

    //[Fact]
    //public void TimerApp_ZwembadAan_VerifyCalls()
    //{
    //    // Arrange
    //    ResetAllMocks();
    //    Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(7).AddMinutes(59).ToUniversalTime().Ticks);
    //    HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.SwitchZwembad.EntityId), null));


    //    // Act
    //    var app = Context.GetApp<TimerApp>();
    //    Scheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);

    //    // Assert
    //    VerifyAllMocks();
    //}

    //[Fact]
    //public void TimerApp_ZwembadOff_VerifyCalls()
    //{
    //    // Arrange
    //    ResetAllMocks();

    //    Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(19).AddMinutes(59).ToUniversalTime().Ticks);
    //    HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.SwitchZwembad.EntityId), null));


    //    // Act
    //    var app = Context.GetApp<TimerApp>();
    //    Scheduler.AdvanceBy(TimeSpan.FromHours(2).Ticks);

    //    // Assert
    //    VerifyAllMocks();
    //}

    [Fact]
    public void TimerApp_VacuumHoliday_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputText.Housestate.EntityId, new EntityState() { State = "Holiday" });
        HaMock.Setup(x => x.CallService("vacuum", "set_fan_speed", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Vacuum.DreameP20294b09RobotCleaner.EntityId), It.IsAny<object>()));
        HaMock.Setup(x => x.CallService("vacuum", "start", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Vacuum.DreameP20294b09RobotCleaner.EntityId), null));
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(20).AddMinutes(59).ToUniversalTime().Ticks);

        // Act
        var app = Context.GetApp<TimerApp>();
        Scheduler.AdvanceBy(TimeSpan.FromHours(1).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_VacuumNoHoliday_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputText.Housestate.EntityId, new EntityState() { State = "Awake" });
        Scheduler.AdvanceTo(DateTime.Now.Date.AddHours(20).AddMinutes(59).ToUniversalTime().Ticks);

        // Act
        var app = Context.GetApp<TimerApp>();
        Scheduler.AdvanceBy(TimeSpan.FromHours(1).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_PowerTariffLow_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Sensor.PowerTariff, "unknown");
        HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.LyncLader.EntityId), null));


        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerStateChange(Entities.Sensor.PowerTariff, "low");


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_PowerTariffHigh_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Sensor.PowerTariff, "unknown");
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.LyncLader.EntityId), null));


        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerStateChange(Entities.Sensor.PowerTariff, "normal");


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerbeddenFinishedEvent_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        var @event = JsonSerializer.Deserialize<Event>(sleeptimerbeddenFinishedEvent)!;
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.BedGreet.EntityId), null));
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.BedKen.EntityId), null));

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerEvent(@event);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerbeddenkidsFinishedEvent_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        var @event = JsonSerializer.Deserialize<Event>(sleeptimerbeddenkidsFinishedEvent)!;
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.BedDamon.EntityId), null));
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.BedCaitlyn.EntityId), null));

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerEvent(@event);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerkidsFinishedEvent_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        var @event = JsonSerializer.Deserialize<Event>(sleeptimerkidsFinishedEvent)!;
        HaMock.Setup(x => x.CallService("light", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Light.Slaapkamerkids.EntityId), It.IsAny<LightTurnOffParameters>()));

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerEvent(@event);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerbeddenkidsChanged0_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("timer", "cancel", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Timer.Sleeptimerbeddenkids.EntityId), null));

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerStateChange(Entities.InputNumber.Sleeptimerbeddenminuteskids, "0");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerbeddenkidsChanged10_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("timer", "start", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Timer.Sleeptimerbeddenkids.EntityId), It.Is<TimerStartParameters>(x => x.Duration == "600")));

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerStateChange(Entities.InputNumber.Sleeptimerbeddenminuteskids, "10");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerChanged0_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("timer", "cancel", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Timer.Sleeptimerbedden.EntityId), null));

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerStateChange(Entities.InputNumber.Sleeptimerbeddenminutes, "0");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerChanged10_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("timer", "start", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Timer.Sleeptimerbedden.EntityId), It.Is<TimerStartParameters>(x => x.Duration == "600")));

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerStateChange(Entities.InputNumber.Sleeptimerbeddenminutes, "10");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerKidsChanged0_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("timer", "cancel", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Timer.Sleeptimerkids.EntityId), null));
        HaMock.Setup(x => x.CallService("light", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Light.Slaapkamerkids.EntityId), It.IsAny<LightTurnOffParameters>()));

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerStateChange(Entities.InputNumber.Sleeptimerkids, "0");

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_SleeptimerKidsChanged10_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("light", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Light.Slaapkamerkids.EntityId), It.IsAny<LightTurnOffParameters>()));
        HaMock.Setup(x => x.CallService("timer", "start", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Timer.Sleeptimerkids.EntityId), It.Is<TimerStartParameters>(x => x.Duration == "600")));
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(y => y.EntityId == Entities.Light.LightSlaapkamerkids2.EntityId), 20, "red")).Returns(false);

        // Act
        var sut = Context.GetApp<TimerApp>();
        HaMock.TriggerStateChange(Entities.InputNumber.Sleeptimerkids, "10");

        // Assert
        VerifyAllMocks();
    }
}