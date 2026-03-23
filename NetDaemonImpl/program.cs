using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDaemon.Extensions.Logging;
using NetDaemon.Extensions.Scheduler;
using NetDaemon.Extensions.Tts;
using NetDaemon.Runtime;
using NetDaemonImpl.IObservable;
using NetDaemonImpl.Modules;
using NetDaemonImpl.Modules.Notify;
using NetDaemonInterface;
using NetDaemonInterface.Models;
using NetDaemonInterface.Observable;
using System.Reflection;

/*
 * dotnet tool update -g NetDaemon.HassModel.CodeGen
 */

try
{
    Console.WriteLine("Starting v4.43");
    await Host.CreateDefaultBuilder(args)
        .UseNetDaemonAppSettings()
        .UseNetDaemonDefaultLogging()
        .UseNetDaemonRuntime()
        .UseNetDaemonTextToSpeech()
        .ConfigureServices((hostContext, services) =>
        {
            services
                .AddAppsFromAssembly(Assembly.GetExecutingAssembly())
                .AddNetDaemonStateManager()
                .AddNetDaemonScheduler();
            // Bind Frigate settings from configuration
            services.Configure<FrigateSettings>(hostContext.Configuration.GetSection("Frigate"));
            services.Configure<ThinginoSettings>(hostContext.Configuration.GetSection("Thingino"));
            services.AddSingleton<IDelayProvider, DelayProvider>();
            services.AddSingleton<ILightControl, LightControl>();
            services.AddSingleton<ILuxBasedBrightness, LuxBasedBrightness>();
            services.AddSingleton<INotify, Notify>();
            services.AddSingleton<IHouseNotificationImageCreator, HouseNotificationImageCreator>();
            services.AddSingleton<ISettingsProvider, SettingsProvider>();
            services.AddSingleton<IButtonEvents, ButtonEvents>();
            services.AddSingleton<IHouseStateEvents, HouseStateEvents>();
            services.AddSingleton<IDayNightEvents, DayNightEvents>();
            services.AddSingleton<IFrigateClient, FrigateClient>();
            services.AddSingleton<IThinginoClient, ThinginoClient>();
        })
        .Build()
        .RunAsync()
        .ConfigureAwait(false);
}
catch (Exception e)
{
    Console.WriteLine($"Failed to start host... {e}");
    throw;
}