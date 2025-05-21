using Nop.Core.Infrastructure;
using Nop.Services.Themes;

namespace NopStation.Plugin.Theme.NopGadget.Services
{
    public class NopGadgetThemeProvider : IThemeProvider
    {
        private readonly INopFileProvider _fileProvider;

        public NopGadgetThemeProvider(INopFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public async Task<IList<ThemeDescriptor>> GetThemesAsync()
        {
            var themeList = new List<ThemeDescriptor>();

            try
            {
                var themePath = _fileProvider.MapPath("~/Themes/NopGadget");
                var themeJsonPath = Path.Combine(themePath, "theme.json");

                if (_fileProvider.FileExists(themeJsonPath))
                {
                    var themeDescriptor = new ThemeDescriptor
                    {
                        FriendlyName = "NopGadget",
                        SystemName = "NopGadget",
                        PreviewImageUrl = "~/Themes/NopGadget/preview.jpg",
                        PreviewText = "NopGadget responsive theme",
                        SupportRtl = true
                    };

                    themeList.Add(themeDescriptor);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading theme: {ex.Message}");
            }

            return await Task.FromResult(themeList);
        }

        public async Task<ThemeDescriptor> GetThemeBySystemNameAsync(string systemName)
        {
            var themes = await GetThemesAsync();
            return themes.FirstOrDefault(t => t.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<bool> ThemeExistsAsync(string systemName)
        {
            var themePath = _fileProvider.MapPath($"~/Themes/{systemName}/theme.json");
            return await Task.FromResult(_fileProvider.FileExists(themePath));
        }


        public ThemeDescriptor GetThemeDescriptorFromText(string text)
        {
            // You can customize this based on your logic
            return new ThemeDescriptor
            {
                FriendlyName = "NopGadget",
                SystemName = "NopGadget",
                PreviewImageUrl = "~/Themes/NopGadget/preview.jpg",
                PreviewText = text,
                SupportRtl = true
            };
        }
    }
}
