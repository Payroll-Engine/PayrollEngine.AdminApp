using System;
using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Payroll Engine database service
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Get the current database version
    /// </summary>
    /// <param name="connection">Database connection</param>
    Task<Version> GetCurrentVersionAsync(DatabaseConnection connection);

    /// <summary>
    /// Get the database status
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="version">Working version</param>
    Task<DatabaseStatus> GetStatusAsync(DatabaseConnection connection, Version version = null);

    /// <summary>
    /// Create a new database
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="collation">Database collation</param>
    /// <returns>Error code, zero on success</returns>
    Task<int?> CreateDatabaseAsync(DatabaseConnection connection, string collation = null);

    /// <summary>
    /// Execute database script
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="script">Database script</param>
    /// <remarks>Script is executed within a database transaction</remarks>
    /// <returns>Number of affected rows, -1 on error</returns>
    Task<int?> ExecuteScriptAsync(DatabaseConnection connection, string script);
}