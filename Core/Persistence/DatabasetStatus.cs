
namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Database status
/// </summary>
public enum DatabaseStatus
{
    /// <summary>
    /// Undefined database connection
    /// </summary>
    UndefinedConnection,

    /// <summary>
    /// Invalid database connection
    /// </summary>
    InvalidConnection,

    /// <summary>
    /// Database server not available
    /// </summary>
    MissingServer,

    /// <summary>
    /// No database present
    /// </summary>
    MissingDatabase,

    /// <summary>
    /// Database is empty
    /// </summary>
    EmptyDatabase,

    /// <summary>
    /// Outdated database version
    /// </summary>
    OutdatedVersion,

    /// <summary>
    /// Database available
    /// </summary>
    Available
}