
namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Asset parameter
/// </summary>
public interface IAssetParameter
{
    /// <summary>
    /// Validate the asset
    /// </summary>
    /// <remarks>Throws an exception on invalid parameter</remarks>
    void Validate();
}