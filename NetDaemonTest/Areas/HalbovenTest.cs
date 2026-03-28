using Moq;
using NetDaemonImpl.apps.Areas;
using NetDaemonInterface.Enums;
using NetDaemonInterface.Observable;
using NetDaemonTest.Apps.Helpers;
using System.Reactive.Subjects;
using Xunit;

namespace NetDaemonTest.Areas;

public class HalbovenTest : TestBase
{
    private readonly Subject<ButtonEvent> buttonSubject = new();

    private void SetupMocks()
    {
        DeconzButtonEventsMock.Setup(x => x.Event).Returns(buttonSubject);
    }

    [Fact]
    public void Halboven_Constructor_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();

        // Act
        var app = Context.GetApp<Halboven>();

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Halboven_SingleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.HalBoven.EntityId), 1, 255)).Returns(true);
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Single, It.Is<LightEntity>(l => l.EntityId == Entities.Light.HalBovenZij.EntityId), 1, 255)).Returns(true);

        var app = Context.GetApp<Halboven>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.HalBoven1, ButtonEventType.Single, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Halboven_DoubleClick_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.HalBoven.EntityId), 1, 255)).Returns(true);
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.Double, It.Is<LightEntity>(l => l.EntityId == Entities.Light.HalBovenZij.EntityId), 1, 255)).Returns(true);

        var app = Context.GetApp<Halboven>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.HalBoven2, ButtonEventType.Double, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }

    [Fact]
    public void Halboven_LongPress_VerifyCalls()
    {
        // Arrange
        ResetAllMocks();
        SetupMocks();
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.HalBoven.EntityId), 1, 255)).Returns(true);
        LightControlMock.Setup(x => x.ButtonDefaultLuxBased(ButtonEventType.LongPress, It.Is<LightEntity>(l => l.EntityId == Entities.Light.HalBovenZij.EntityId), 1, 255)).Returns(true);

        var app = Context.GetApp<Halboven>();

        // Act
        buttonSubject.OnNext(new ButtonEvent(Button.HalBoven3, ButtonEventType.LongPress, DateTime.MinValue));

        // Assert
        VerifyAllMocks();
    }
}
