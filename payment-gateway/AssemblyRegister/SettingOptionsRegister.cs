using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace payment_gateway.AssemblyRegister
{
    internal static class SettingOptionsRegister
    {
        internal static void RegisterSettingOptions<TSettings>(this IServiceCollection service, IConfiguration configuration) where TSettings : class, new()
        {
            service.Configure<TSettings>(configuration.GetSection(typeof(TSettings).Name));
        }
    }
}
