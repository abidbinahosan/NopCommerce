﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using NopStation.Plugin.Misc.Core.Infrastructure;

namespace NopStation.Plugin.Theme.Kingdom.Infrastructure;

public class PluginNopStartup : INopStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddNopStationServices("NopStation.Plugin.Theme.Kingdom");
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 10;
}