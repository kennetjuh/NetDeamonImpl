using NetDaemonInterface;

namespace NetDaemonImpl.apps;

[NetDaemonApp]
public class IdleSettingApp : MyNetDaemonBaseApp
{
    public IdleSettingApp(IHaContext haContext, IScheduler scheduler, ILogger<IdleSettingApp> logger, ISettingsProvider settingsProvider)
        : base(haContext, scheduler, logger, settingsProvider)
    {
        _entities.Vacuum.DreameP20294b09RobotCleaner.StateChanges()
            .Throttle(TimeSpan.FromMinutes(5), scheduler)
            .Where(x => x.New?.State == "docked")
            .Subscribe(x => _entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Basic"));

        if (_entities.Vacuum.DreameP20294b09RobotCleaner.State == "docked")
        {
            _entities.Vacuum.DreameP20294b09RobotCleaner.SetFanSpeed("Basic");
        }

        _entities.MediaPlayer.Speelkamer.StateChanges()
           .Throttle(TimeSpan.FromSeconds(5), scheduler)
           .Where(x => x.New?.State == "off")
           .Subscribe(x => _entities.MediaPlayer.Speelkamer.VolumeSet(0.7));

        if (_entities.MediaPlayer.Speelkamer.State == "off")
        {
            _entities.MediaPlayer.Speelkamer.VolumeSet(0.7);
        }

        _entities.MediaPlayer.Woonkamer.StateChanges()
            .Throttle(TimeSpan.FromSeconds(5), scheduler)
            .Where(x => x.New?.State == "off")
            .Subscribe(x => _entities.MediaPlayer.Woonkamer.VolumeSet(0.7));

        if (_entities.MediaPlayer.Woonkamer.State == "off")
        {
            _entities.MediaPlayer.Woonkamer.VolumeSet(0.7);
        }
    }
}