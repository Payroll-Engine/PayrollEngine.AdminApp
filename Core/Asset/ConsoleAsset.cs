using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Console asset
/// </summary>
public class ConsoleAsset : AssetBase
{
    /// <inheritdoc />
    public override Task UpdateStatusAsync(AssetContext context) =>
        Task.CompletedTask;
}