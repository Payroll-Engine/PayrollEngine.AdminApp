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
        // file assets root folder
        var configRoot = Configuration["FileAssetsRoot"];
        if (configRoot == null || !OperatingSystem.DirectoryExists(configRoot))
        {
            configRoot = ".";
        }

        // app directory: use parent directory in case of local start
        var appDirectory = OperatingSystem.GetAppDirectory();
        if (string.Equals(configRoot, appDirectory))
        {
            configRoot = OperatingSystem.GetCurrentParentDirectory();
        }

        return configRoot;
    }

    #endregion

}