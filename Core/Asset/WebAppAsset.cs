using System.Threading.Tasks;
using System.Collections.Generic;
using PayrollEngine.AdminApp.WebServer;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Web app asset
/// </summary>
public class WebAppAsset : WebServerAssetBase, IStartAsset
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
        // refresh web server status in base class
        await base.UpdateStatusAsync(context);

        // asset not available
        if (!Available)
        {
            WebAppStatus = WebAppStatus.NotAvailable;
            return;
        }

        // backend server not defined
        if (WebServerConnection.IsEmpty())
        {
            WebAppStatus = WebAppStatus.WebServerUndefined;
            return;
        }

        // backend server not available
        if (WebServerStatus != WebServerStatus.Available)
        {
            WebAppStatus = WebAppStatus.WebServerNotStarted;
            return;
        }

        WebAppStatus = WebAppStatus.Running;
    }

    /// <inheritdoc />
    public Task StartAsync()
    {
        OperatingSystem.StartWebServer(Name, Parameters.WebServerExec, WebServerConnection.ToUrl());
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task LoadAsync(AssetContext context, Dictionary<string, object> parameters = null)
    {
        // web server connection from environment
        var connection = await context.SettingsService.GetWebAppConnectionAsync();
        if (connection != null)
        {
            WebServerConnection.ImportValues(connection);
        }

        // parameters
        Parameters = LoadParameters<WebAppAssetParameters>(parameters);

        await base.LoadAsync(context, parameters);
    }
}