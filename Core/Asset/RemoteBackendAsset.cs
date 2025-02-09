using System.Threading.Tasks;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Remote backend asset
/// </summary>
public class RemoteBackendAsset : WebserverAssetBase, IVirtualAsset
{
    /// <summary>
    /// Constructor
    /// </summary>
    public RemoteBackendAsset()
    {
        // always available
        Available = true;
    }

    /// <inheritdoc />
    public override async Task LoadAsync(AssetContext context, Dictionary<string, object> parameters = null)
    {
        // webserver connection from environment
        var connection = await context.SettingsService.GetApiConnectionAsync();
        if (connection != null)
        {
            WebserverConnection.ImportValues(connection);
        }
        await base.LoadAsync(context, parameters);
    }
}