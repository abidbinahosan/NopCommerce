using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;  // Add this NuGet package if not already installed
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Themes;

namespace Nop.Plugin.Themes.NopGadget.Infrastructure
{
    public class ThemeProvider : IThemeProvider
    {
        private readonly INopFileProvider _fileProvider;

        public ThemeProvider(INopFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public Task<IList<ThemeDescriptor>> GetThemesAsync()
        {
            var themeDescriptors = new List<ThemeDescriptor>();
            var themeRootPath = _fileProvider.Combine("Themes");

            if (!_fileProvider.DirectoryExists(themeRootPath))
                return Task.FromResult((IList<ThemeDescriptor>)themeDescriptors);

            var directories = _fileProvider.GetDirectories(themeRootPath);

            foreach (var directory in directories)
            {
                var themeDescriptorPath = _fileProvider.Combine(directory, "theme.json");

                if (!_fileProvider.FileExists(themeDescriptorPath))
                    continue;

                var text = _fileProvider.ReadAllText(themeDescriptorPath, Encoding.UTF8);
                var themeDescriptor = GetThemeDescriptorFromText(text);

                if (themeDescriptor != null)
                    themeDescriptors.Add(themeDescriptor);
            }

            return Task.FromResult((IList<ThemeDescriptor>)themeDescriptors);
        }

        public async Task<ThemeDescriptor> GetThemeBySystemNameAsync(string systemName)
        {
            var themes = await GetThemesAsync();
            return themes.FirstOrDefault(td => td.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        public async Task<bool> ThemeExistsAsync(string systemName)
        {
            var themes = await GetThemesAsync();
            return themes.Any(td => td.SystemName.Equals(systemName, StringComparison.InvariantCultureIgnoreCase));
        }

        public ThemeDescriptor GetThemeDescriptorFromText(string text)
        {
            try
            {
                return JsonConvert.DeserializeObject<ThemeDescriptor>(text);
            }
            catch (Exception)
            {
                // optionally log or handle invalid theme.json content here
                return null;
            }
        }
    }
}
