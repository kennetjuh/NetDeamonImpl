using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Reactive.Testing;
using Moq;
using NetDaemon.Client;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonInterface.Models;
using NetDaemonInterface.Observable;
using System.Reactive.Concurrency;
using System.Text.Json;

namespace NetDaemonTest.Apps.Helpers;

public class TestContext : IServiceProvider
{
    private readonly IServiceCollection _serviceCollection = new ServiceCollection();
    private readonly IServiceProvider _serviceProvider;

    public TestContext()
    {
        _serviceCollection.AddGeneratedCode();
        _serviceCollection.AddHomeAssistantGenerated();
        _serviceCollection.AddSingleton(_ => new HaContextMock(MockBehavior.Strict));
        _serviceCollection.AddTransient<IHaContext>(s => s.GetRequiredService<HaContextMock>().Object);
        _serviceCollection.AddSingleton<TestScheduler>();
        _serviceCollection.AddTransient<IScheduler>(s => s.GetRequiredService<TestScheduler>());

        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NotifyApp>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NotifyApp>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NotifyHandlerApp>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NotifyHandlerApp>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<TestApp>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<TestApp>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<TimerApp>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<TimerApp>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<HouseApp>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<HouseApp>>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<IDelayProvider>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<IDelayProvider>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<ILightControl>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILightControl>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<ILuxBasedBrightness>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILuxBasedBrightness>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<INotify>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<INotify>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<IHouseNotificationImageCreator>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<IHouseNotificationImageCreator>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<ISettingsProvider>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ISettingsProvider>>().Object);

        _serviceCollection.AddScoped(_ => new Mock<IButtonEvents>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<IButtonEvents>>().Object);

        _serviceCollection.AddScoped(_ => new Mock<IDayNightEvents>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<IDayNightEvents>>().Object);

        _serviceCollection.AddScoped(_ => new Mock<IHouseStateEvents>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<IHouseStateEvents>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<IHomeAssistantRunner>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<IHomeAssistantRunner>>().Object);

        _serviceCollection.AddSingleton(_ =>
        {
            var mock = new Mock<ITriggerManager>();
            mock.Setup(x => x.RegisterTrigger(It.IsAny<object>()))
                .Returns(System.Reactive.Linq.Observable.Empty<JsonElement>());
            return mock;
        });
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ITriggerManager>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<IFrigateClient>(MockBehavior.Strict));
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<IFrigateClient>>().Object);

        _serviceCollection.AddSingleton(_ =>
        {
            var mock = new Mock<IThinginoClient>(MockBehavior.Strict);
            mock.Setup(x => x.Connect(It.IsAny<string>()));
            return mock;
        });
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<IThinginoClient>>().Object);

        _serviceCollection.AddSingleton(Options.Create(new ThinginoSettings()));

        _serviceCollection.AddSingleton(_ => new Mock<ILogger<Camera>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<Camera>>>().Object);

        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.Badkamer>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.Badkamer>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.Eetkamer>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.Eetkamer>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.Garage>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.Garage>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.Halboven>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.Halboven>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.Inkom>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.Inkom>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.Kelder>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.Kelder>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.Keuken>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.Keuken>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.KippenHok>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.KippenHok>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.Rommelkamer>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.Rommelkamer>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.SlaapkamerCaitlyn>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.SlaapkamerCaitlyn>>>().Object);
        _serviceCollection.AddSingleton(_ => new Mock<ILogger<NetDaemonImpl.apps.Areas.SlaapkamerKen>>());
        _serviceCollection.AddTransient(s => s.GetRequiredService<Mock<ILogger<NetDaemonImpl.apps.Areas.SlaapkamerKen>>>().Object);

        _serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    public T GetApp<T>() => ActivatorUtilities.GetServiceOrCreateInstance<T>(_serviceProvider);

    public Entities Entities => this.GetRequiredService<Entities>();
    public HaContextMock HaMock => this.GetRequiredService<HaContextMock>();
}