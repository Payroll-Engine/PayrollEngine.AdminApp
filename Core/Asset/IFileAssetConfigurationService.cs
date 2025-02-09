namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Payroll Engine file asset configuration service
/// </summary>
public interface IFileAssetConfigurationService
{
    /// <summary>
    /// Get file asset root path
    /// </summary>
    string GetRoot();
}