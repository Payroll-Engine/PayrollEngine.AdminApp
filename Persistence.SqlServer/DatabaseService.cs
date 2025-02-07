using System;
using System.Linq;
using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Persistence.SqlServer;

/// <summary>
/// SQL Server database service
/// </summary>
/// <param name="errorService">Error service</param>
public class DatabaseService(IErrorService errorService) : IDatabaseService
{
    private IErrorService ErrorService { get; } = errorService;

    /// <inheritdoc />
    public async Task<Version> GetCurrentVersionAsync(DatabaseConnection connection)
    {
        // invalid connection
        if (await DatabaseTool.TestDatabaseAvailableAsync(connection, ErrorService) != true)
        {
            return null;
        }

        // empty database
        var tableAvailable = await DatabaseTool.TestTableAvailableAsync(connection, DatabaseVersion.TableName, ErrorService);
        if (tableAvailable != true)
        {
            return null;
        }

        // outdated database
        var versions = await DatabaseTool.GetDatabaseVersionsAsync(connection, ErrorService);
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

        // database available
        var dbAvailable = await DatabaseTool.TestDatabaseAvailableAsync(connection, ErrorService);
        if (dbAvailable != true)
        {
            // sql server available
            var serverVersion = DatabaseTool.GetServerVersion(connection.Server, ErrorService);
            if (serverVersion == null)
            {
                return DatabaseStatus.MissingServer;
            }

            // missing database
            return DatabaseStatus.MissingDatabase;
        }

        // empty database
        var tableAvailable = await DatabaseTool.TestTableAvailableAsync(connection, DatabaseVersion.TableName, ErrorService);
        if (tableAvailable != true)
        {
            return DatabaseStatus.EmptyDatabase;
        }

        // version check
        if (version != null)
        {
            // outdated database
            var versions = await DatabaseTool.GetDatabaseVersionsAsync(connection, ErrorService);
            if (versions.Max() < version)
            {
                return DatabaseStatus.OutdatedVersion;
            }
        }

        // available database
        return DatabaseStatus.Available;
    }

    /// <inheritdoc />
    public async Task<int?> CreateDatabaseAsync(DatabaseConnection connection, string collation = null) =>
        await DatabaseTool.CreateDatabaseAsync(connection, ErrorService, collation);

    /// <inheritdoc />
    public async Task<int?> ExecuteScriptAsync(DatabaseConnection connection, string script) =>
        await DatabaseTool.ExecuteScriptAsync(connection, script, ErrorService);
}