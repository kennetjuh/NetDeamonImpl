using Microsoft.Extensions.DependencyInjection;

namespace NetDaemonTest.Apps.Helpers
{
    public static class Extensions
    {
        public static IServiceCollection AddGeneratedCode(this IServiceCollection serviceCollection)
            => serviceCollection
                .AddTransient<Entities>()
                .AddTransient<Services>();
    }
}
