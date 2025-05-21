using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Services.Themes;
using NopStation.Plugin.Theme.NopGadget.Services;

namespace NopStation.Plugin.Theme.NopGadget.Infrastructure
{
    public class NopStartup : INopStartup
    {
        public int Order => 3000;

        public void Configure(IApplicationBuilder application)
        {
            // Nothing to configure
        }

        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register your theme provider
            services.AddScoped<IThemeProvider, NopGadgetThemeProvider>();
        }
    }
}