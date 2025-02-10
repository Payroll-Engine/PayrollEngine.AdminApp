using System.Threading.Tasks;
using System.Collections.Generic;
using PayrollEngine.AdminApp.Webserver;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Backend asset (local)
/// </summary>
public class BackendAsset : WebserverAssetBase, IStartAsset
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

        // refresh webserver status in base class
        await base.UpdateStatusAsync(context);

        // asset not available
        if (Parameters.Database == null || !Available)
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

        // backend status: webserver not defined
        if (WebserverStatus == WebserverStatus.UndefinedConnection)
        {
            BackendStatus = BackendStatus.WebserverUndefined;
            return;
        }

        // backend status: webserver not started
        if (WebserverStatus == WebserverStatus.NotAvailable)
        {
            BackendStatus = BackendStatus.WebserverNotStarted;
            return;
        }

        // backend status: available
        BackendStatus = BackendStatus.Running;
    }

    /// <inheritdoc />
    public Task StartAsync()
    {
        OperatingSystem.StartWebserver(
            workingDirectory: Name,
            webserverExec: Parameters.WebserverExec,
            webserverUrl: WebserverConnection.ToUrl(),
            webserverName: Parameters.WebserverName);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public override async Task LoadAsync(AssetContext context, Dictionary<string, object> parameters = null)
    {
        // webserver connection from environment
        var webserverConnection = await context.SettingsService.GetApiConnectionAsync();
        if (webserverConnection != null)
        {
            WebserverConnection.ImportValues(webserverConnection);
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