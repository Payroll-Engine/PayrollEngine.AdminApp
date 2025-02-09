using System.Threading.Tasks;
using PayrollEngine.AdminApp.Webserver;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Webserver asset
/// </summary>
public abstract class WebserverAssetBase : AssetBase, IBrowseAsset
{
    /// <summary>
    /// Webserver connection
    /// </summary>
    public WebserverConnection WebserverConnection { get; } = new();

    /// <summary>
    /// Webserver status
    /// </summary>
    public WebserverStatus WebserverStatus { get; private set; }

    /// <inheritdoc />
    public override async Task UpdateStatusAsync(AssetContext context)
    {
        // asset not available
        if (!Available)
        {
            WebserverStatus = WebserverStatus.NotAvailable;
            return;
        }

        // refresh the webserver status
        WebserverStatus = await context.WebserverService.GetStatusAsync(WebserverConnection);
    }

    /// <inheritdoc />
    public Task BrowseAsync()
    {
        var url = WebserverConnection.ToUrl();
        if (!string.IsNullOrWhiteSpace(url))
        {
            OperatingSystem.StartProcess(url);
        }
        return Task.CompletedTask;
    }
}