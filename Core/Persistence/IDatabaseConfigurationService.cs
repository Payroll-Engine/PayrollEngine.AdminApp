namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Payroll Engine admin database configuration service
/// </summary>
public interface IDatabaseConfigurationService
{
    /// <summary>
    /// Get connection timeout in seconds
    /// </summary>
    int GetConnectionTimeout();
}