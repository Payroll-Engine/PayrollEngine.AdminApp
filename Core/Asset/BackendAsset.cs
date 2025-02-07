using System.Threading.Tasks;
using System.Collections.Generic;
using PayrollEngine.AdminApp.WebServer;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Backend asset (local)
/// </summary>
public class BackendAsset : WebServerAssetBase, IStartAsset
{
    /// <summary>
    /// Asset parameters
    /// </summary>
    public BackendAssetParameters Parameters { get; private set; } = new();

    /// <summary>
    /// Backend status
    /// </summary>
    public BackendStatus BackendStatus { get; private set; }

    /// <summary>
    /// Backend database connection
    /// </summary>
    public DatabaseConnection DatabaseConnection { get; } = new();

    /// <summary>
    /// Backend database status
    /// </summary>
    public DatabaseStatus DatabaseStatus { get; private set; }

    /// <inheritdoc />
    public override async Task UpdateStatusAsync(AssetContext context)
    {
        // refresh web server status in base class
        await base.UpdateStatusAsync(context);

        // asset not available
        if (!Available)
        {
            BackendStatus = BackendStatus.NotAvailable;
            return;
        }

        // refresh database status
        DatabaseStatus = await context.DatabaseService.GetStatusAsync(
            connection: DatabaseConnection,
            version: Parameters.Database.CurrentVersion);

        // backend status: database not available
        if (DatabaseStatus <= DatabaseStatus.EmptyDatabase)
        {
            BackendStatus = BackendStatus.DatabaseNotAvailable;
            return;
        }

        // backend status: web server not defined
        if (WebServerStatus == WebServerStatus.UndefinedConnection)
        {
            BackendStatus = BackendStatus.WebServerUndefined;
            return;
        }

        // backend status: web server not started
        if (WebServerStatus == WebServerStatus.NotAvailable)
        {
            BackendStatus = BackendStatus.WebServerNotStarted;
            return;
        }

        // backend status: available
        BackendStatus = BackendStatus.Running;
    }

    /// <inheritdoc />
    public Task StartAsync()
    {
        OperatingSystem.StartWebServer(Name, Parameters.WebServerExec, WebServerConnection.ToUrl());
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task LoadAsync(AssetContext context, Dictionary<string, object> parameters = null)
    {
        // web server connection from environment
        var webServerConnection = await context.SettingsService.GetApiConnectionAsync();
        if (webServerConnection != null)
        {
            WebServerConnection.ImportValues(webServerConnection);
        }

        // load database connection from environment
        var dbConnection = await context.SettingsService.GetDatabaseConnectionAsync();
        if (dbConnection != null)
        {
            DatabaseConnection.ImportValues(dbConnection);
        }

        // parameters
        Parameters = LoadParameters<BackendAssetParameters>(parameters);

        await base.LoadAsync(context, parameters);
    }
}