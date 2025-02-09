using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;

namespace PayrollEngine.AdminApp.Persistence.SqlServer;

/// <summary>
/// SQL Server tool
/// </summary>
internal static class DatabaseTool
{
    /// <summary>
    /// Get the SQL Server version
    /// </summary>
    /// <param name="serverName">Server name</param>
    /// <param name="errorService">Error service</param>
    internal static Version GetServerVersion(string serverName, IErrorService errorService = null)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(serverName))
            {
                return null;
            }

            ServerVersion version;
            if (IsLocalServer(serverName))
            {
                version = new Server().PingSqlServerVersion(serverName);
            }
            else
            {
                var server = new Server(serverName);
                version = server.PingSqlServerVersion(serverName);
            }
            if (version == null)
            {
                return null;
            }
            return new(version.Major, version.Minor, version.BuildNumber);
        }
        catch (Exception exception)
        {
            errorService?.AddError(exception);
            Debug.WriteLine(exception.GetBaseException().Message);
            return null;
        }
    }

    /// <summary>
    /// Test for available database
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="errorService">Error service</param>
    internal static async Task<bool?> TestDatabaseAvailableAsync(DatabaseConnection connection,
        IErrorService errorService = null)
    {
        if (!connection.HasRequiredValues())
        {
            return null;
        }

        try
        {
            var database = await GetDatabaseAsync(connection);
            return database != null;
        }
        catch (Exception exception)
        {
            errorService?.AddError(exception);
            Debug.WriteLine(exception.GetBaseException().Message);
            return null;
        }
    }

    /// <summary>
    /// Test for empty database
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="errorService">Error service</param>
    internal static async Task<bool?> TestEmptyDatabaseAsync(DatabaseConnection connection,
        IErrorService errorService = null)
    {
        try
        {
            var database = await GetDatabaseAsync(connection);
            if (database == null)
            {
                return null;
            }
            return database.Tables.Count == 0;
        }
        catch (Exception exception)
        {
            errorService?.AddError(exception);
            Debug.WriteLine(exception.GetBaseException().Message);
            return null;
        }
    }

    /// <summary>
    /// Get the database versions
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="errorService">Error service</param>
    /// <returns>List of versions</returns>
    internal static async Task<List<Version>> GetDatabaseVersionsAsync(DatabaseConnection connection,
        IErrorService errorService = null)
    {
        var versions = new List<Version>();

        try
        {
            var connectionString = connection.ToConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }

            await using var sqlConnection = new SqlConnection(connectionString);
            const string query = $"SELECT {nameof(DatabaseVersion.MajorVersion)}, " +
                                    $"{nameof(DatabaseVersion.MinorVersion)}, " +
                                    $"{nameof(DatabaseVersion.SubVersion)} " +
                                 $"FROM {DatabaseVersion.TableName}";
            await using (sqlConnection)
            {
                await using var command = new SqlCommand(query, sqlConnection);
                await sqlConnection.OpenAsync();
                await using var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    var major = reader.GetInt32(0);
                    var minor = reader.GetInt32(1);
                    var sub = reader.GetInt32(2);
                    versions.Add(new Version(major, minor, sub));
                }
                return versions;
            }
        }
        catch (Exception exception)
        {
            errorService?.AddError(exception);
            Debug.WriteLine(exception.GetBaseException().Message);
            return null;
        }
    }

    /// <summary>
    /// Create a database
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="collation">Database collation</param>
    /// <param name="errorService">Error service</param>
    internal static async Task<bool?> CreateDatabaseAsync(DatabaseConnection connection,
        string collation = null, IErrorService errorService = null)
    {
        // invalid connection
        if (!connection.HasRequiredValues())
        {
            return null;
        }

        // database
        try
        {
            // connection
            var connectionString = connection.ToConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return null;
            }
            await using var sqlConnection = new SqlConnection(connectionString);
            var serverConnection = new ServerConnection(sqlConnection);

            // server
            var server = IsLocalServer(connection.Server) ?
                // local
                new Server() :
                // remote
                new Server(serverConnection);

            // database
            var database = new Database(server, connection.Database);
            if (!string.IsNullOrWhiteSpace(collation))
            {
                database.Collation = collation;
            }

            database.Create();
            return true;
        }
        catch (Exception exception)
        {
            errorService?.AddError(exception);
            Debug.WriteLine(exception.GetBaseException().Message);
            return null;
        }
    }

    /// <summary>
    /// Execute database script
    /// </summary>
    /// <param name="connection">Database connection</param>
    /// <param name="script">Database script</param>
    /// <param name="errorService">Error service</param>
    /// <remarks>Script is executed within a database transaction</remarks>
    /// <returns>Number of affected rows, -1 on error</returns>
    internal static async Task<int?> ExecuteScriptAsync(DatabaseConnection connection,
        string script, IErrorService errorService = null)
    {
        if (string.IsNullOrWhiteSpace(script))
        {
            throw new ArgumentException(nameof(script));
        }

        // ensure database context
        script = $"USE [{connection.Database}]{Environment.NewLine}{script}";

        // connection
        var connectionString = connection.ToConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return -1;
        }
        await using var sqlConnection = new SqlConnection(connectionString);
        var serverConnection = new ServerConnection(sqlConnection);

        try
        {
            // server
            var server = new Server(serverConnection);

            // start transaction (see https://stackoverflow.com/a/48020723)
            serverConnection.BeginTransaction();

            // query execute
            var result = server.ConnectionContext.ExecuteNonQuery(script);

            // commit transaction
            serverConnection.CommitTransaction();

            return result;
        }
        catch (Exception exception)
        {
            // rollback transaction
            serverConnection.RollBackTransaction();

            // error handling
            errorService?.AddError(exception);
            Debug.WriteLine(exception.GetBaseException().Message);
            return -1;
        }
    }

    #region Helper

    /// <summary>
    /// Test for local database server
    /// </summary>
    /// <param name="server">Server name</param>
    private static bool IsLocalServer(string server) =>
        Environment.MachineName.Equals(new Server(server).Name, StringComparison.InvariantCultureIgnoreCase);

    /// <summary>
    /// Get database object
    /// </summary>
    /// <param name="connection">Database connection</param>
    private static async Task<Database> GetDatabaseAsync(DatabaseConnection connection)
    {
        // connection
        var connectionString = connection.ToConnectionString();
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return null;
        }
        await using var sqlConnection = new SqlConnection(connectionString);
        var serverConnection = new ServerConnection(sqlConnection);

        var server = new Server(serverConnection);
        foreach (Database database in server.Databases)
        {
            if (string.Equals(database.Name, connection.Database, StringComparison.InvariantCultureIgnoreCase))
            {
                return database;
            }
        }
        return null;
    }

    #endregion

}