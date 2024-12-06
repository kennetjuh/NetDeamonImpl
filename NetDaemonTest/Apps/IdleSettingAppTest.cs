using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps;
using NetDaemonTest.Apps.Helpers;
using System.Linq;
using Xunit;

namespace NetDaemonTest.Apps;

public class IdleSettingAppTest : TestBase
{
    [Fact]
    public void IdleSettingsApp_Constructor_NoEvents()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Vacuum.DreameP20294b09RobotCleaner, "unknown");
        HaMock.TriggerStateChange(Entities.MediaPlayer.Speelkamer, "unknown");
        HaMock.TriggerStateChange(Entities.MediaPlayer.Woonkamer, "unknown");

        // Act
        var app = Context.GetApp<IdleSettingApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void IdleSettingsApp_Constructor_AllEvents()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Vacuum.DreameP20294b09RobotCleaner, "docked");
        HaMock.TriggerStateChange(Entities.MediaPlayer.Speelkamer, "off");
        HaMock.TriggerStateChange(Entities.MediaPlayer.Woonkamer, "off");

        HaMock.Setup(x => x.CallService("vacuum", "set_fan_speed",
            It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Vacuum.DreameP20294b09RobotCleaner.EntityId),
            It.Is<VacuumSetFanSpeedParameters>(x => x.FanSpeed == "Basic")));
        HaMock.Setup(x => x.CallService("media_player", "volume_set",
            It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.MediaPlayer.Speelkamer.EntityId),
            It.Is<MediaPlayerVolumeSetParameters>(x => x.VolumeLevel == 0.7)));
        HaMock.Setup(x => x.CallService("media_player", "volume_set",
            It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.MediaPlayer.Woonkamer.EntityId),
            It.Is<MediaPlayerVolumeSetParameters>(x => x.VolumeLevel == 0.7)));

        // Act
        var app = Context.GetApp<IdleSettingApp>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void IdleSettingsApp_IdleDetectionVacuum_VerifyMocks()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.Vacuum.DreameP20294b09RobotCleaner, "unknown");
        HaMock.Setup(x => x.CallService("vacuum", "set_fan_speed",
            It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.Vacuum.DreameP20294b09RobotCleaner.EntityId),
            It.Is<VacuumSetFanSpeedParameters>(x => x.FanSpeed == "Basic")));

        // Act
        var app = Context.GetApp<IdleSettingApp>();
        HaMock.TriggerStateChange(Entities.Vacuum.DreameP20294b09RobotCleaner, "docked");
        Scheduler.AdvanceBy(TimeSpan.FromMinutes(5).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void IdleSettingsApp_IdleDetectionHal_VerifyMocks()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.MediaPlayer.Speelkamer, "unknown");
        HaMock.Setup(x => x.CallService("media_player", "volume_set",
            It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.MediaPlayer.Speelkamer.EntityId),
            It.Is<MediaPlayerVolumeSetParameters>(x => x.VolumeLevel == 0.7)));

        // Act
        var app = Context.GetApp<IdleSettingApp>();
        HaMock.TriggerStateChange(Entities.MediaPlayer.Speelkamer, "off");
        Scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void IdleSettingsApp_IdleDetectionWoonkamer_VerifyMocks()
    {
        // Arrange
        ResetAllMocks();
        HaMock.TriggerStateChange(Entities.MediaPlayer.Woonkamer, "unknown");
        HaMock.Setup(x => x.CallService("media_player", "volume_set",
            It.Is<ServiceTarget>(x => x.EntityIds!.SingleOrDefault()! == Entities.MediaPlayer.Woonkamer.EntityId),
            It.Is<MediaPlayerVolumeSetParameters>(x => x.VolumeLevel == 0.7)));

        // Act
        var app = Context.GetApp<IdleSettingApp>();
        HaMock.TriggerStateChange(Entities.MediaPlayer.Woonkamer, "off");
        Scheduler.AdvanceBy(TimeSpan.FromSeconds(5).Ticks);

        // Assert
        VerifyAllMocks();
    }
}