using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Reactive.Testing;
using Moq;
using NetDaemonImpl.apps;
using NetDaemonInterface;
using NetDaemonInterface.Observable;
using System.Reactive.Concurrency;

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


        _serviceProvider = _serviceCollection.BuildServiceProvider();
    }

    public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

    public T GetApp<T>() => ActivatorUtilities.GetServiceOrCreateInstance<T>(_serviceProvider);

    public Entities Entities => this.GetRequiredService<Entities>();
    public HaContextMock HaMock => this.GetRequiredService<HaContextMock>();
}