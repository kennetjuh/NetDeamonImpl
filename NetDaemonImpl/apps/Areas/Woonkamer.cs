using NetDaemonInterface;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NetDaemonImpl.apps.Areas;

[NetDaemonApp]
public class Woonkamer : AreaBase
{
    private readonly LightEntity bank;
    private readonly LightEntity bureau;
    private readonly LightEntity nis;
    private readonly SwitchEntity kast;
    private const double minBrightness = 1;
    private const double maxBrightness = 255;
    private List<CoverEntity> GordijnEntities;
    private List<double> LastCoverPositions;
    private bool CoverMoving;
    private bool CoverDirectionUp;
    readonly Timer timer1;

    public Woonkamer(IHaContext haContext, IScheduler scheduler, ILogger<Badkamer> logger,
        ISettingsProvider settingsProvider, IButtonEvents deconzButtonEvents, IDelayProvider delayProvider, ILightControl lightControl, IHouseStateEvents houseStateEvents)
        : base(haContext, scheduler, logger, settingsProvider, delayProvider, lightControl, deconzButtonEvents, houseStateEvents)
    {
        bank = _entities.Light.WoonkamerBank;
        bureau = _entities.Light.WoonkamerBureau;
        nis = _entities.Light.WoonkamerNis;
        kast = _entities.Switch.PlugWoonkamerKast;
        lightControl.AddMaxWhiteLight(bureau);
        lightControl.AddMaxWhiteLight(bank);
        GordijnEntities = [_entities.Cover.WoonkamerGordijn1, _entities.Cover.WoonkamerGordijn2, _entities.Cover.WoonkamerGordijn3, _entities.Cover.WoonkamerGordijn4   ];
        LastCoverPositions = GetCoverPositions();
        timer1 = new Timer((x) => CheckCoverMovement(), null, 0, 1000);

        SubscribeToDeconzButton(Button.Gordijn);
        SubscribeToDeconzButton(Button.Bank);
        SubscribeToDeconzButton(Button.Woonkamer);
    }

    private void CheckCoverMovement()
    {
        var newPositions = GetCoverPositions();

        var movementDetected = false;
        for (int i = 0; i < LastCoverPositions.Count; i++)
        {
            if (LastCoverPositions[i] != newPositions[i])
            {
                movementDetected = true;
                CoverDirectionUp = newPositions.Average() > LastCoverPositions.Average();
                break;
            }
        }
        CoverMoving = movementDetected;

        //Console.WriteLine($"Moving: {CoverMoving} | Current:  {string.Join(',', LastCoverPositions)} | New: {string.Join(',', newPositions)} | movingup: {newPositions.Average() > LastCoverPositions.Average()}");

        LastCoverPositions = [.. newPositions];
    }

    private List<double> GetCoverPositions()
    {
        List<double> positions = [];
        foreach (var gordijn in GordijnEntities)
        {
            positions.Add(gordijn.Attributes?.CurrentPosition ?? 0);
        }
        return positions;
    }

    public override void ButtonPressed(ButtonEvent buttonEvent)
    {
        if (buttonEvent.Button == Button.Gordijn)
        {
            StopAfterTask();
            if (CoverMoving)
            {
                Stop();
                return;
            }

            switch (buttonEvent.Event)
            {
                case ButtonEventType.up_open:
                    if (GetCoverPositions().Average() == 90)
                    {
                        Move(100);
                    }
                    else
                    {
                        Move(90);
                    }
                    break;
                case ButtonEventType.down_close:
                    Move(25);
                    break;                
                default:
                    // because the release event is not stable we ignore it for now.
                    break;

            }

            void Move(double? position)
            {
                StopAfterTask();
                if (position == null)
                {
                    return;
                }
                foreach (var gordijn in GordijnEntities)
                {
                    gordijn.SetCoverPosition((long)position);
                }
            }

            void Stop()
            {
                StopAfterTask();

                foreach (var gordijn in GordijnEntities)
                {
                    gordijn.StopCover();
                }
                StartAfterTask(TimeSpan.FromMilliseconds(2000), () =>
                {
                    var positions = GetCoverPositions();
                    Move(CoverDirectionUp ? positions.Max() : positions.Min());
                });
            }
        }
        else if (buttonEvent.Button == Button.Bank)
        {
            switch (buttonEvent.Event)
            {
                case ButtonEventType.Single:
                        lightControl.SetLight(nis, 100);
                        _entities.Switch.PlugWoonkamerKast.TurnOn();
                    if ((DateTime.Now - buttonEvent.previousEvent) > TimeSpan.FromSeconds(2))
                    {
                        lightControl.SetLight(bank, 0);
                        lightControl.SetLight(bureau, 0); 
                    }
                    else
                    {
                        lightControl.SetLight(bank, minBrightness);
                        lightControl.SetLight(bureau, minBrightness);
                    }
                    break;
                case ButtonEventType.Double:
                case ButtonEventType.LongPress:
                    lightControl.ButtonDefault(buttonEvent.Event, nis);
                    break;
                default:
                    break;
            }
        }
        else
        {
            lightControl.ButtonDefaultLuxBased(buttonEvent.Event, bank, minBrightness, maxBrightness);
            lightControl.ButtonDefaultLuxBased(buttonEvent.Event, bureau, minBrightness, maxBrightness);
        }
    }
}
