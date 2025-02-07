using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Asset wit browse support
/// </summary>
public interface IBrowseAsset
{
    /// <summary>
    /// Start asset browse
    /// </summary>
    Task BrowseAsync();
}