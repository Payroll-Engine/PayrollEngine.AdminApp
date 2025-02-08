using System;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Persistence.SqlServer;

/// <summary>
/// SQL Server database service
/// </summary>
/// <param name="errorService">Error service</param>
/// <param name="timeout">Request timeout</param>
/// <param name="collation">Database collation</param>
public class DatabaseService(IErrorService errorService, int timeout, string collation) : IDatabaseService
{
    private IErrorService ErrorService { get; } = errorService;
    private int Timeout { get; } = timeout;
    private string Collation { get; } = collation;

    /// <summary>
    /// Default connection timeout
    /// </summary>
    public static readonly int DefaultTimeout = 5;

    /// <inheritdoc />
    public async Task<Version> GetCurrentVersionAsync(DatabaseConnection connection)
    {
        var testConnection = ToTestConnection(connection);

        // invalid connection
        if (await DatabaseTool.TestDatabaseAvailableAsync(testConnection, ErrorService) != true)
        {
            return null;
        }

        // empty database
        var tableEmpty = await DatabaseTool.TestEmptyDatabaseAsync(testConnection, ErrorService);
        if (tableEmpty == true)
        {
            return null;
        }

        // outdated database
        var versions = await DatabaseTool.GetDatabaseVersionsAsync(testConnection, ErrorService);
        return versions.Max();
    }

    /// <inheritdoc />
    public async Task<DatabaseStatus> GetStatusAsync(DatabaseConnection connection, Version version = null)
    {
        // empty connection
        if (connection.IsEmpty())
        {
            return DatabaseStatus.UndefinedConnection;
        }

        // invalid connection
        if (!connection.HasRequiredValues())
        {
            return DatabaseStatus.InvalidConnection;
        }

        var testConnection = ToTestConnection(connection);

        // database available
        var dbAvailable = await DatabaseTool.TestDatabaseAvailableAsync(testConnection, ErrorService);
        if (dbAvailable != true)
        {
            // sql server available
            var serverVersion = DatabaseTool.GetServerVersion(testConnection.Server, ErrorService);
            if (serverVersion == null)
            {
                return DatabaseStatus.MissingServer;
            }

            // missing database
            return DatabaseStatus.MissingDatabase;
        }

        // empty database
        var tableEmpty = await DatabaseTool.TestEmptyDatabaseAsync(testConnection, ErrorService);
        if (tableEmpty == true)
        {
            return DatabaseStatus.EmptyDatabase;
        }

        // version check
        if (version != null)
        {
            // outdated database
            var versions = await DatabaseTool.GetDatabaseVersionsAsync(testConnection, ErrorService);
            if (versions.Max() < version)
            {
                return DatabaseStatus.OutdatedVersion;
            }
        }

        // available database
        return DatabaseStatus.Available;
    }

    /// <inheritdoc />
    public async Task<int?> CreateDatabaseAsync(DatabaseConnection connection) =>
        await DatabaseTool.CreateDatabaseAsync(ToTestConnection(connection), ErrorService, Collation);

    /// <inheritdoc />
    public async Task<bool?> IsEmptyDatabaseAsync(DatabaseConnection connection) =>
        await DatabaseTool.TestEmptyDatabaseAsync(ToTestConnection(connection), ErrorService);

    /// <inheritdoc />
    public async Task<int?> ExecuteScriptAsync(DatabaseConnection connection, string script) =>
        await DatabaseTool.ExecuteScriptAsync(ToTestConnection(connection), script, ErrorService);
    
    /// <summary>
    /// Convert the database connection to a connection string, reducing the request timeout
    /// </summary>
    /// <param name="connection">Database connection</param>
    private DatabaseConnection ToTestConnection(DatabaseConnection connection)
    {
        if (Timeout == 0)
        {
            return connection;
        }
        // apply custom timeout
        return new DatabaseConnection(connection) { Timeout = Timeout };
    }
}