using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonTest.Apps.Helpers;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Apps;

public class CabineAppTest : TestBase
{
    [Fact]
    public void TimerApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();

        // Act
        var app = Context.GetApp<CabineApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_ActualNull_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputNumber.Cabinetemptarget.EntityId, new EntityState() { State = "20" });
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = "15" });
        HaMock.TriggerStateChange(Entities.InputBoolean.Cabinethermostat.EntityId, new EntityState() { State = "on" });
        NotifyMock.Setup(x => x.NotifyGsmKen("CabineThermostat", "Actual or Target is NULL", NotifyPriorityEnum.high, null, It.IsAny<List<NotifyActionEnum>>()));

        // Act
        var app = Context.GetApp<CabineApp>();
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = null });


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void TimerApp_TargetNull_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputNumber.Cabinetemptarget.EntityId, new EntityState() { State = null });
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = "15" });
        HaMock.TriggerStateChange(Entities.InputBoolean.Cabinethermostat.EntityId, new EntityState() { State = "on" });
        NotifyMock.Setup(x => x.NotifyGsmKen("CabineThermostat", "Actual or Target is NULL", NotifyPriorityEnum.high, null, It.IsAny<List<NotifyActionEnum>>()));


        // Act
        var app = Context.GetApp<CabineApp>();
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = "16" });


        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CabineApp_ThermostatSwitchToOn_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = "15" });
        HaMock.TriggerStateChange(Entities.InputNumber.Cabinetemptarget.EntityId, new EntityState() { State = "20" });
        HaMock.TriggerStateChange(Entities.InputBoolean.Cabinethermostat.EntityId, new EntityState() { State = "off" });
        HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.CabineHeater.EntityId), It.IsAny<object>()));

        // Act
        var app = Context.GetApp<CabineApp>();
        HaMock.TriggerStateChange(Entities.InputBoolean.Cabinethermostat.EntityId, new EntityState() { State = "on" });

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CabineApp_ThermostatSwitchToOff_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.CabineHeater.EntityId), It.IsAny<object>()));

        // Act
        var app = Context.GetApp<CabineApp>();
        HaMock.TriggerStateChange(Entities.InputBoolean.Cabinethermostat.EntityId, new EntityState() { State = "off" });

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CabineApp_ThermostatChangeTarget_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = "15" });
        HaMock.TriggerStateChange(Entities.InputNumber.Cabinetemptarget.EntityId, new EntityState() { State = "15" });
        HaMock.TriggerStateChange(Entities.InputBoolean.Cabinethermostat.EntityId, new EntityState() { State = "on" });
        HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.CabineHeater.EntityId), It.IsAny<object>()));

        // Act
        var app = Context.GetApp<CabineApp>();
        HaMock.TriggerStateChange(Entities.InputNumber.Cabinetemptarget.EntityId, new EntityState() { State = "18" });

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void CabineApp_ThermostatOff_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = "15" });
        HaMock.TriggerStateChange(Entities.InputNumber.Cabinetemptarget.EntityId, new EntityState() { State = "20" });
        HaMock.TriggerStateChange(Entities.InputBoolean.Cabinethermostat.EntityId, new EntityState() { State = "off" });

        // Act
        var app = Context.GetApp<CabineApp>();
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = "21" });

        // Assert
        VerifyAllMocks();
    }

    [Theory]
    [InlineData(19.9, 20, true)]
    [InlineData(20.1, 20, false)]
    [InlineData(1, 20, true)]
    [InlineData(100, 20, false)]
    public void CabineApp_ThermostatOn_VerifyCalls(double actual, double target, bool result)
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.InputBoolean.Cabinethermostat.EntityId, new EntityState() { State = "on" });
        HaMock.TriggerStateChange(Entities.InputNumber.Cabinetemptarget.EntityId, new EntityState() { State = target.ToString(System.Globalization.CultureInfo.InvariantCulture) });

        if (result)
        {
            HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.CabineHeater.EntityId), It.IsAny<object>()));
        }
        else
        {
            HaMock.Setup(x => x.CallService("switch", "turn_off", It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Switch.CabineHeater.EntityId), It.IsAny<object>()));
        }

        // Act
        var app = Context.GetApp<CabineApp>();
        HaMock.TriggerStateChange(Entities.Sensor.MultiCabineTemp.EntityId, new EntityState() { State = actual.ToString(System.Globalization.CultureInfo.InvariantCulture) });

        // Assert
        VerifyAllMocks();
    }
}