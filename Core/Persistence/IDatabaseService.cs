using System;
using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Payroll Engine admin database service
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// Get the current database version
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="errorService">Error service</param>
    Task<Version> GetCurrentVersionAsync(DatabaseConnection connection, IErrorService errorService = null);

    /// <summary>
    /// Get the database status
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="version">Working version</param>
    /// <param name="errorService">Error service</param>
    Task<DatabaseStatus> GetStatusAsync(DatabaseConnection connection, Version version = null, IErrorService errorService = null);

    /// <summary>
    /// Create a new database
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="collation">Database collation</param>
    /// <param name="errorService">Error service</param>
    /// <returns>True on success</returns>
    Task<bool?> CreateDatabaseAsync(DatabaseConnection connection, string collation = null, IErrorService errorService = null);

    /// <summary>
    /// Execute database script
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="script">Database script</param>
    /// <param name="errorService">Error service</param>
    /// <remarks>Script is executed within a database transaction</remarks>
    /// <returns>Number of affected rows, -1 on error</returns>
    Task<int?> ExecuteScriptAsync(DatabaseConnection connection, string script, IErrorService errorService = null);
}