using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetDaemon.Extensions.Logging;
using NetDaemon.Extensions.Scheduler;
using NetDaemon.Extensions.Tts;
using NetDaemon.Runtime;
using NetDaemonImpl.Modules;
using NetDaemonImpl.Modules.Notify;
using NetDaemonInterface;
using System.Reflection;

#pragma warning disable CA1812

try
{
    Console.WriteLine("Starting v0.0.2");
    await Host.CreateDefaultBuilder(args)
        .UseNetDaemonAppSettings()
        .UseNetDaemonDefaultLogging()
        .UseNetDaemonRuntime()
        .UseNetDaemonTextToSpeech()
        .ConfigureServices((_, services) =>
        {
            services
                .AddAppsFromAssembly(Assembly.GetExecutingAssembly())
                .AddNetDaemonStateManager()
                .AddNetDaemonScheduler();
            services.AddSingleton<IAreaCollection, AreaCollection>();
            services.AddSingleton<IDelayProvider, DelayProvider>();
            services.AddSingleton<IHouseState, HouseState>();
            services.AddSingleton<ILightControl, LightControl>();
            services.AddSingleton<ILuxBasedBrightness, LuxBasedBrightness>();
            services.AddSingleton<INotify, Notify>();
            services.AddSingleton<ITwinkle, Twinkle>();
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