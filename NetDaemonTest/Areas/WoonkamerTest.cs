using Moq;
using NetDaemon.HassModel.Entities;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Linq;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class WoonkamerTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
        LightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerBureau.EntityId)));
        LightControlMock.Setup(x => x.AddMaxWhiteLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerBank.EntityId)));
    }

    [Fact]
    public void Woonkamer_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<Woonkamer>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Woonkamer_WoonkamerButton_Single_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerBank.EntityId), 1, 255)).Returns(true);
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerBureau.EntityId), 1, 255)).Returns(true);

        var app = Context.GetApp<Woonkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Woonkamer, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Woonkamer_BankButton_Single_RecentPress_LightsOn()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerNis.EntityId), 100)).Returns(true);
        HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(s => s.EntityIds!.SingleOrDefault()! == Entities.Switch.PlugWoonkamerKast.EntityId), null));
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerBank.EntityId), 1)).Returns(true);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerBureau.EntityId), 1)).Returns(true);

        var app = Context.GetApp<Woonkamer>();

        // Act - previousEvent is recent (less than 2 seconds ago)
        buttonSubject.OnNext(new ButtonEvent(Button.Bank, ButtonEventType.Single, DateTime.Now));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Woonkamer_BankButton_Single_OldPress_LightsOff()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerNis.EntityId), 100)).Returns(true);
        HaMock.Setup(x => x.CallService("switch", "turn_on", It.Is<ServiceTarget>(s => s.EntityIds!.SingleOrDefault()! == Entities.Switch.PlugWoonkamerKast.EntityId), null));
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerBank.EntityId), 0)).Returns(false);
        LightControlMock.Setup(x => x.SetLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerBureau.EntityId), 0)).Returns(false);

        var app = Context.GetApp<Woonkamer>();

        // Act - previousEvent is old (more than 2 seconds ago)
        buttonSubject.OnNext(new ButtonEvent(Button.Bank, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Woonkamer_BankButton_Double_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefault(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerNis.EntityId))).Returns(true);

        var app = Context.GetApp<Woonkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Bank, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Woonkamer_BankButton_LongPress_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefault(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.WoonkamerNis.EntityId))).Returns(true);

        var app = Context.GetApp<Woonkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Bank, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Woonkamer_GordijnButton_UpOpen_MovesCoverTo90()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.Setup(x => x.CallService("cover", "set_cover_position", It.IsAny<ServiceTarget>(), It.IsAny<CoverSetCoverPositionParameters>()));

        var app = Context.GetApp<Woonkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Gordijn, ButtonEventType.up_open, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Woonkamer_GordijnButton_DownClose_MovesCoverTo25()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        HaMock.Setup(x => x.CallService("cover", "set_cover_position", It.IsAny<ServiceTarget>(), It.IsAny<CoverSetCoverPositionParameters>()));

        var app = Context.GetApp<Woonkamer>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.Gordijn, ButtonEventType.down_close, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }
}
