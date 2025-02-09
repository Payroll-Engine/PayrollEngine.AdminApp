using System.Threading.Tasks;
using System.Collections.Generic;
using PayrollEngine.AdminApp.Webserver;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Web app asset
/// </summary>
public class WebAppAsset : WebserverAssetBase, IStartAsset
{
    /// <summary>
    /// Asset parameters
    /// </summary>
    private WebAppAssetParameters Parameters { get; set; } = new();

    /// <summary>
    /// Web app status
    /// </summary>
    public WebAppStatus WebAppStatus { get; private set; }

    /// <inheritdoc />
    public override async Task UpdateStatusAsync(AssetContext context)
    {
        // refresh webserver status in base class
        await base.UpdateStatusAsync(context);

        // asset not available
        if (!Available)
        {
            WebAppStatus = WebAppStatus.NotAvailable;
            return;
        }

        // backend server not defined
        if (WebserverConnection.IsEmpty())
        {
            WebAppStatus = WebAppStatus.WebserverUndefined;
            return;
        }

        // backend server not available
        if (WebserverStatus != WebserverStatus.Available)
        {
            WebAppStatus = WebAppStatus.WebserverNotStarted;
            return;
        }

        WebAppStatus = WebAppStatus.Running;
    }

    /// <inheritdoc />
    public Task StartAsync()
    {
        OperatingSystem.StartWebserver(
            workingDirectory: Name,
            webserverExec: Parameters.WebserverExec,
            webserverUrl: WebserverConnection.ToUrl(),
            webserverName: Parameters.WebserverName);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task LoadAsync(AssetContext context, Dictionary<string, object> parameters = null)
    {
        // webserver connection from environment
        var connection = await context.SettingsService.GetWebAppConnectionAsync();
        if (connection != null)
        {
            WebserverConnection.ImportValues(connection);
        }

        // parameters
        Parameters = LoadParameters<WebAppAssetParameters>(parameters);

        await base.LoadAsync(context, parameters);
    }
}