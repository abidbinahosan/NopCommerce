using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Themes.NopGadget.Areas.Admin.Infrastructure
{
    public class AdminStartup : INopStartup
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Register custom Razor view locations for Admin area
            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.AreaViewLocationFormats.Add("/Plugins/Theme.NopGadget/Areas/Admin/Views/{1}/{0}.cshtml");
                options.AreaViewLocationFormats.Add("/Plugins/Theme.NopGadget/Areas/Admin/Views/Shared/{0}.cshtml");
            });
        }

        public void Configure(IApplicationBuilder application)
        {
            // No middleware needed for theme area
        }

        public int Order => 1000; // Run late to avoid conflicts
    }
}
