using Nop.Core.Caching;

namespace NopStation.Plugin.Theme.Kingdom.Infrastructure.Cache;

public class ModelCacheKey
{
    public static CacheKey TopMenuModelKey => new CacheKey("Nopstation.theme.kingdom.topmenu-{0}-{1}-{2}", TopMenuModelPattern);
    public static string TopMenuModelPattern => "Nopstation.theme.kingdom.topmenu";

    public static CacheKey FooterDescriptionModelKey => new CacheKey("Nopstation.theme.kingdom.footer.description-{0}-{1}-{2}", FooterDescriptionModelPattern);
    public static string FooterDescriptionModelPattern => "Nopstation.theme.kingdom.footer.description";

    public static CacheKey SocialModelKey => new CacheKey("Nopstation.theme.kingdom.social-{0}-{1}-{2}", SocialModelPattern);
    public static string SocialModelPattern => "Nopstation.theme.kingdom.social";

}
