using Microsoft.Extensions.DependencyInjection;

namespace Septem.Utils.Helpers.DateTime;

public static class ServiceCollectionExtensions
{
    public static void AddAcbServices(this IServiceCollection services)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
    }
}