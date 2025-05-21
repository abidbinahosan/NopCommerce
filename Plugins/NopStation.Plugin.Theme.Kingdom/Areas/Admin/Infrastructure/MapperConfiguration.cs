using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using NopStation.Plugin.Theme.Kingdom.Areas.Admin.Models;

namespace NopStation.Plugin.Theme.Kingdom.Areas.Admin.Infrastructure;

public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    public int Order => 1;

    public MapperConfiguration()
    {
        CreateMap<KingdomSettings, ConfigurationModel>()
                .ForMember(model => model.ActiveStoreScopeConfiguration, options => options.Ignore());
        CreateMap<ConfigurationModel, KingdomSettings>();
    }
}
