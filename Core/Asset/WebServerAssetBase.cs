using System.Threading.Tasks;
using PayrollEngine.AdminApp.WebServer;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Web server asset
/// </summary>
public abstract class WebServerAssetBase : AssetBase, IBrowseAsset
{
    /// <summary>
    /// Web server connection
    /// </summary>
    public WebServerConnection WebServerConnection { get; } = new();

    /// <summary>
    /// Web server status
    /// </summary>
    public WebServerStatus WebServerStatus { get; private set; }

    /// <inheritdoc />
    public override async Task UpdateStatusAsync(AssetContext context)
    {
        // asset not available
        if (!Available)
        {
            WebServerStatus = WebServerStatus.NotAvailable;
            return;
        }

        // refresh the web server status
        WebServerStatus = await context.WebServerService.GetStatusAsync(WebServerConnection);
    }

    /// <inheritdoc />
    public Task BrowseAsync()
    {
        var url = WebServerConnection.ToUrl();
        if (!string.IsNullOrWhiteSpace(url))
        {
            OperatingSystem.StartProcess(url);
        }
        return Task.CompletedTask;
    }
}