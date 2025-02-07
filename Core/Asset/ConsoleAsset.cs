using System.Threading.Tasks;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Console asset
/// </summary>
public class ConsoleAsset : AssetBase
{
    /// <summary>
    /// Asset parameters
    /// </summary>
    public ConsoleAssetParameters Parameters { get; private set; } = new();

    /// <inheritdoc />
    public override Task UpdateStatusAsync(AssetContext context) =>
        Task.CompletedTask;

    /// <inheritdoc />
    public override async Task LoadAsync(AssetContext context, Dictionary<string, object> parameters = null)
    {
        // parameters
        Parameters = LoadParameters<ConsoleAssetParameters>(parameters);

        await base.LoadAsync(context, parameters);
    }
}