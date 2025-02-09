using System;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Persistence.SqlServer;

/// <summary>
/// SQL Server database service
/// </summary>
public class DatabaseService : IDatabaseService
{
    /// <summary>
    /// SQL Server database service
    /// </summary>
    /// <param name="configService">Configuration service</param>
    public DatabaseService(IDatabaseConfigurationService configService)
    {

        // timeout
        var configTimeout = configService.GetConnectionTimeout();
        if (configTimeout <= 0)
        {
            configTimeout = Specification.HttpConnectTimeoutDefault;
        }
        Timeout = configTimeout;

        // collation
        Collation = configService.GetCollation();
    }

    private int Timeout { get; }
    private string Collation { get; }

    /// <inheritdoc />
    public async Task<Version> GetCurrentVersionAsync(DatabaseConnection connection, IErrorService errorService = null)
    {
        var testConnection = ToTestConnection(connection);

        // invalid connection
        if (await DatabaseTool.TestDatabaseAvailableAsync(testConnection, errorService) != true)
        {
            return null;
        }

        // empty database
        var tableEmpty = await DatabaseTool.TestEmptyDatabaseAsync(testConnection, errorService);
        if (tableEmpty == true)
        {
            return null;
        }

        // outdated database
        var versions = await DatabaseTool.GetDatabaseVersionsAsync(testConnection, errorService);
        return versions.Max();
    }

    /// <inheritdoc />
    public async Task<DatabaseStatus> GetStatusAsync(DatabaseConnection connection, Version version = null,
        IErrorService errorService = null)
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
        var dbAvailable = await DatabaseTool.TestDatabaseAvailableAsync(testConnection, errorService);
        if (dbAvailable != true)
        {
            // sql server available
            var serverVersion = DatabaseTool.GetServerVersion(testConnection.Server, errorService);
            if (serverVersion == null)
            {
                return DatabaseStatus.MissingServer;
            }

            // missing database
            return DatabaseStatus.MissingDatabase;
        }

        // empty database
        var tableEmpty = await DatabaseTool.TestEmptyDatabaseAsync(testConnection, errorService);
        if (tableEmpty == true)
        {
            return DatabaseStatus.EmptyDatabase;
        }

        // version check
        if (version != null)
        {
            // outdated database
            var versions = await DatabaseTool.GetDatabaseVersionsAsync(testConnection, errorService);
            if (versions.Max() < version)
            {
                return DatabaseStatus.OutdatedVersion;
            }
        }

        // available database
        return DatabaseStatus.Available;
    }

    /// <inheritdoc />
    public async Task<bool?> CreateDatabaseAsync(DatabaseConnection connection, IErrorService errorService = null) =>
        await DatabaseTool.CreateDatabaseAsync(ToTestConnection(connection), Collation, errorService);

    /// <inheritdoc />
    public async Task<int?> ExecuteScriptAsync(DatabaseConnection connection, string script, IErrorService errorService = null) =>
        await DatabaseTool.ExecuteScriptAsync(ToTestConnection(connection), script, errorService);

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