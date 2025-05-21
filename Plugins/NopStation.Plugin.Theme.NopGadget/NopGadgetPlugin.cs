using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core;
using Nop.Services.Plugins;
using Nop.Core.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace NopStation.Plugin.Theme.NopGadget
{
    public class NopGadgetPlugin : BasePlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly IServiceProvider _serviceProvider;
        private readonly INopFileProvider _fileProvider;

        // Add proper dependency injection for all required services
        public NopGadgetPlugin(
            IWebHelper webHelper,
            IServiceProvider serviceProvider,
            INopFileProvider fileProvider)
        {
            _webHelper = webHelper;
            _serviceProvider = serviceProvider;
            _fileProvider = fileProvider;
        }

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ThemeNopGadget/Configure";
        }

        public override async Task InstallAsync()
        {
            try
            {
                // Log before migration
                var logBuilder = new StringBuilder();
                logBuilder.AppendLine("Starting theme installation...");

                // Run migrations
                using var scope = _serviceProvider.CreateScope();
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                logBuilder.AppendLine("Running migrations...");
                runner.MigrateUp();
                logBuilder.AppendLine("Migrations completed successfully.");

                // Get correct plugin path using nopCommerce's file provider
                var pluginPath = _fileProvider.MapPath("~/Plugins/NopStation.Plugin.Theme.NopGadget/Themes/NopGadget");
                logBuilder.AppendLine($"Plugin path: {pluginPath}");

                // Get correct destination path
                var themePath = _fileProvider.MapPath("~/Themes/NopGadget");
                logBuilder.AppendLine($"Theme destination path: {themePath}");

                // Ensure the destination directory exists
                if (!_fileProvider.DirectoryExists(themePath))
                {
                    _fileProvider.CreateDirectory(themePath);
                    logBuilder.AppendLine("Created destination directory.");
                }

                // Copy theme files
                CopyDirectory(pluginPath, themePath, logBuilder);
                logBuilder.AppendLine("Theme files copied successfully.");

                // Write installation log to a file for debugging
                var logPath = _fileProvider.MapPath("~/App_Data/plugin_install_log.txt");
                _fileProvider.WriteAllText(logPath, logBuilder.ToString(), Encoding.UTF8);

                await base.InstallAsync();
                logBuilder.AppendLine("Base installation completed.");
            }
            catch (Exception ex)
            {
                // Log the exception to a file
                var errorLogPath = _fileProvider.MapPath("~/App_Data/plugin_install_error.txt");
                var errorMessage = $"Error installing NopGadget theme: {ex.Message}\r\n{ex.StackTrace}";
                if (ex.InnerException != null)
                {
                    errorMessage += $"\r\nInner Exception: {ex.InnerException.Message}\r\n{ex.InnerException.StackTrace}";
                }

                _fileProvider.WriteAllText(errorLogPath, errorMessage, Encoding.UTF8);

                // Re-throw to prevent plugin from being marked as installed when it failed
                throw;
            }
        }

        public override async Task UninstallAsync()
        {
            try
            {
                // Get correct theme path using nopCommerce's file provider
                var themePath = _fileProvider.MapPath("~/Themes/NopGadget");

                // Remove theme directory
                if (_fileProvider.DirectoryExists(themePath))
                {
                    _fileProvider.DeleteDirectory(themePath);
                }

                await base.UninstallAsync();
            }
            catch (Exception ex)
            {
                // Log the exception
                var errorLogPath = _fileProvider.MapPath("~/App_Data/plugin_uninstall_error.txt");
                var errorMessage = $"Error uninstalling NopGadget theme: {ex.Message}\r\n{ex.StackTrace}";
                _fileProvider.WriteAllText(errorLogPath, errorMessage, Encoding.UTF8);

                // Re-throw to prevent plugin from being marked as uninstalled when it failed
                throw;
            }
        }

        private void CopyDirectory(string sourceDir, string destDir, StringBuilder logBuilder)
        {
            try
            {
                // Create destination directory if it doesn't exist
                if (!_fileProvider.DirectoryExists(destDir))
                {
                    _fileProvider.CreateDirectory(destDir);
                    logBuilder.AppendLine($"Created directory: {destDir}");
                }

                // Copy files
                foreach (var file in _fileProvider.GetFiles(sourceDir))
                {
                    var fileName = Path.GetFileName(file);
                    var destFile = Path.Combine(destDir, fileName);

                    // Make sure destination file is writable if it exists
                    if (_fileProvider.FileExists(destFile))
                    {
                        var fileInfo = new FileInfo(destFile);
                        if (fileInfo.IsReadOnly)
                            fileInfo.IsReadOnly = false;
                    }

                    _fileProvider.FileCopy(file, destFile, true);
                    logBuilder.AppendLine($"Copied file: {fileName}");
                }

                // Copy subdirectories recursively
                foreach (var directory in _fileProvider.GetDirectories(sourceDir))
                {
                    var dirName = Path.GetFileName(directory);
                    var destSubDir = Path.Combine(destDir, dirName);
                    CopyDirectory(directory, destSubDir, logBuilder);
                }
            }
            catch (Exception ex)
            {
                logBuilder.AppendLine($"Error copying directory {sourceDir} to {destDir}: {ex.Message}");
                throw; // Re-throw to handle at the higher level
            }
        }
    }
}