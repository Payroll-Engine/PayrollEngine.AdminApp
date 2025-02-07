using System.Threading.Tasks;
using PayrollEngine.AdminApp.WebServer;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Setting;

/// <summary>
/// Asset configuration service
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Get the database connection string
    /// </summary>
    Task<DatabaseConnection> GetDatabaseConnectionAsync();

    /// <summary>
    /// Change the database connection string
    /// </summary>
    /// <param name="connection">Database connection</param>
    Task SetDatabaseConnectionAsync(DatabaseConnection connection);

    /// <summary>
    /// Get the api key
    /// </summary>
    Task<string> GetApiKeyAsync();

    /// <summary>
    /// Change the api key
    /// </summary>
    /// <param name="apiKey">Api key</param>
    Task SetApiKeyAsync(string apiKey);

    /// <summary>
    /// Get the api connection string
    /// </summary>
    Task<WebServerConnection> GetApiConnectionAsync();

    /// <summary>
    /// Change the api connection string
    /// </summary>
    /// <param name="connection">Database connection</param>
    Task SetApiConnectionAsync(WebServerConnection connection);

    /// <summary>
    /// Get the web app connection string
    /// </summary>
    Task<WebServerConnection> GetWebAppConnectionAsync();

    /// <summary>
    /// Change the web app connection string
    /// </summary>
    /// <param name="connection">Database connection</param>
    Task SetWebAppConnectionAsync(WebServerConnection connection);
}