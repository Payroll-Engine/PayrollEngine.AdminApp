using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Asset with start support
/// </summary>
public interface IStartAsset
{
    /// <summary>
    /// Start the asset
    /// </summary>
    // ReSharper disable once UnusedMemberInSuper.Global
    Task StartAsync(AssetContext contextI);
}