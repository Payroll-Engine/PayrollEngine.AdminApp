using Microsoft.Extensions.Configuration;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Webserver;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Extension methods for <see cref="IConfigurationRoot"/>
/// </summary>
public class ServiceConfigurationService :
    IDatabaseConfigurationService,
    IWebserverConfigurationService,
    IFileAssetConfigurationService
{
    /// <summary>
    /// Extension methods for <see cref="IConfigurationRoot"/>
    /// </summary>
    public ServiceConfigurationService(IConfigurationRoot configuration)
    {
        Configuration = configuration;
    }

    private IConfigurationRoot Configuration { get; }

    #region IDatabaseConfigurationService

    /// <inheritdoc />
    int IDatabaseConfigurationService.GetConnectionTimeout()
    {
        var timeout = Configuration["DatabaseConnectTimeout"];
        if (!string.IsNullOrWhiteSpace(timeout) && int.TryParse(timeout, out var seconds))
        {
            return seconds;
        }
        return 0;
    }

    #endregion

    #region IWebserverConfigurationService

    /// <inheritdoc />
    int IWebserverConfigurationService.GetConnectionTimeout()
    {
        var timeout = Configuration["HttpConnectTimeout"];
        if (!string.IsNullOrWhiteSpace(timeout) && int.TryParse(timeout, out var seconds))
        {
            return seconds;
        }
        return 0;
    }

    #endregion

    #region IFileAssetConfigurationService

    /// <inheritdoc />
    string IFileAssetConfigurationService.GetRoot()
    {
        // configuration
        var configRoot = Configuration["FileAssetsRoot"];
        if (!string.IsNullOrWhiteSpace(configRoot))
        {
            if (OperatingSystem.DirectoryExists(configRoot))
            {
                return configRoot;
            }
            throw new AdminException($"Invalid configuration setting 'FileAssetsRoot': {configRoot}");
        }

        // current directory
        var curDirectory = OperatingSystem.GetCurrentDirectory();
        var appDirectory = OperatingSystem.GetAppDirectory();
        // use parent directory in case of local start
        if (string.Equals(curDirectory, appDirectory))
        {
            curDirectory = OperatingSystem.GetCurrentParentDirectory();
        }
        return curDirectory;
    }

    #endregion

}