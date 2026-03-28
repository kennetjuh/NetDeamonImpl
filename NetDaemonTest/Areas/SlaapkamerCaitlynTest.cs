using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Collections.Generic;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class SlaapkamerCaitlynTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
        LightControlMock.Setup(x => x.AddMaxCustomColorLight(It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerCaitlyn.EntityId), It.IsAny<List<int>>()));
    }

    [Fact]
    public void SlaapkamerCaitlyn_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<SlaapkamerCaitlyn>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerCaitlyn_SingleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerCaitlyn.EntityId), 50, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerCaitlyn>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerCaitlyn1, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerCaitlyn_DoubleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerCaitlyn.EntityId), 50, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerCaitlyn>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerCaitlyn2, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void SlaapkamerCaitlyn_LongPress_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.SlaapkamerCaitlyn.EntityId), 50, 255)).Returns(true);

        var app = Context.GetApp<SlaapkamerCaitlyn>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.SlaapkamerCaitlyn3, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }
}
