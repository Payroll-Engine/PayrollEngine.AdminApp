using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.Webserver;
using PayrollEngine.AdminApp.Persistence;
using PayrollEngine.AdminApp.Presentation.Components.Dialogs;

namespace PayrollEngine.AdminApp.Presentation.Components.Assets;

/// <summary>
/// View for the backend asset
/// </summary>
public abstract class BackendAssetViewBase : ComponentBase
{
    /// <summary>
    /// Backend asset
    /// </summary>
    [Parameter] public BackendAsset Asset { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IStatusMessageService StatusMessageService { get; set; }
    [Inject] private IStatusUpdateService StatusUpdateService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IAssetService AssetService { get; set; }

    private ISettingsService SettingsService => AssetService.AssetContext.SettingsService;
    private IDatabaseService DatabaseService => AssetService.AssetContext.DatabaseService;

    /// <summary>
    /// Status updating indicator
    /// </summary>
    protected bool StatusUpdating => StatusUpdateService.Updating;

    #region Backend

    /// <summary>
    /// Webserver url
    /// </summary>
    protected string WebserverUrl =>
        Asset.WebserverConnection.ToUrl();

    /// <summary>
    /// Webserver url style
    /// </summary>
    protected string WebserverUrlStyle =>
        Asset.WebserverStatus == WebserverStatus.Available ?
            "text-decoration: underline" : null;

    /// <summary>
    /// Webserver href
    /// </summary>
    protected MarkupString WebserverHref
    {
        get
        {
            var serverUrl = WebserverUrl;
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                serverUrl = Localizer.UrlUndefined;
            }
            return Asset.WebserverStatus == WebserverStatus.Available ?
                // link
                MarkupTool.ToHref(serverUrl) :
                // no link
                new(serverUrl);
        }
    }

    /// <summary>
    /// Webserver edit text
    /// </summary>
    protected string WebserverEditText =>
        Asset.WebserverStatus switch
        {
            WebserverStatus.UndefinedConnection => Localizer.Add,
            _ => Localizer.Edit
        };

    /// <summary>
    /// Browse webserver asset (open API)
    /// </summary>
    protected async Task BrowseServerAsync()
    {
        try
        {
            await Asset.BrowseAsync();
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.BackendLocalTitle, exception);
        }
    }

    /// <summary>
    /// Start the webserver
    /// </summary>
    protected async Task StartServerAsync()
    {
        try
        {
            if (!await CheckCertificate())
            {
                return;
            }
            await Asset.StartAsync(AssetService.AssetContext);
            await AssetService.InvalidateStatusAsync();
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.BackendLocalTitle, exception);
        }
    }

    private async Task<bool> CheckCertificate()
    {
        // check for .net dev https certificate
        if (!Asset.WebserverConnection.IsLocalSecureConnection() ||
            OperatingSystem.HasLocalSecureDevCertificate())
        {
            // available
            return true;
        }

        // confirm certificate installation
        var dialog = await DialogService.ShowConfirm(
            title: Localizer.BackendLocalTitle,
            markupMessage: Localizer.MissingDotNetDevCertificate);
        if (dialog != true)
        {
            return false;
        }

        // add certificate
        OperatingSystem.AddLocalSecureDevCertificate();
        // wait
        Thread.Sleep(1000);

        // certificate setup failed
        if (!OperatingSystem.HasLocalSecureDevCertificate())
        {
            dialog = await DialogService.ShowConfirm(
                title: Localizer.BackendLocalTitle,
                markupMessage: Localizer.DotNetDevCertificateSetupFailed);
            return dialog == true;
        }

        // successful installed
        return true;
    }

    /// <summary>
    /// Edit the server connection
    /// </summary>
    protected async Task EditServerAsync()
    {
        try
        {
            // working copy
            var editConnection = new WebserverConnection(Asset.WebserverConnection);

            // init
            if (editConnection.IsEmpty())
            {
                editConnection.BaseUrl = Specification.BackendDefaultBaseUrl;
                editConnection.Port = Specification.BackendDefaultPort;
                editConnection.Timeout = TimeSpan.FromSeconds(Specification.BackendDefaultTimeout);
            }

            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(WebserverConnectionDialog.Connection), editConnection }
            };

            // show dialog
            var dialog = await (await DialogService.ShowAsync<WebserverConnectionDialog>(
                title: Localizer.WebserverDialogTitle, parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            // no changes
            if (editConnection.EqualValues(Asset.WebserverConnection))
            {
                StatusMessageService.SetMessage(Localizer.NoEditChangesMessage);
                return;
            }

            // store user settings
            await SettingsService.SetApiConnectionAsync(editConnection);
            // api key storage (only on backend)
            if (!string.IsNullOrWhiteSpace(editConnection.ApiKey))
            {
                await SettingsService.SetApiKeyAsync(editConnection.ApiKey);
            }

            // update local values
            Asset.WebserverConnection.ImportValues(editConnection);

            // invalidate assets status
            await AssetService.InvalidateStatusAsync();

            // user notification
            StatusMessageService.SetMessage(Localizer.WebserverConnectionUpdateMessage);
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.Webserver, exception);
        }
    }

    #endregion

    #region Database

    /// <summary>
    /// Database connection UI text
    /// </summary>
    protected string DatabaseConnectionUI =>
        $"{Asset.DatabaseConnection.Server} > {Asset.DatabaseConnection.Database}";

    /// <summary>
    /// Database connection edit text
    /// </summary>
    protected string ConnectionEditText =>
        Asset.DatabaseStatus switch
        {
            DatabaseStatus.UndefinedConnection => Localizer.Add,
            _ => Localizer.Edit
        };

    /// <summary>
    /// Database connection setup text
    /// </summary>
    protected string DatabaseSetupText
    {
        get
        {
            if (Asset.DatabaseStatus.ReadyToCreate())
            {
                return Localizer.Create;
            }
            return Asset.DatabaseStatus.ReadyToUpdate() ?
                Localizer.Update :
                Localizer.Edit;
        }
    }

    /// <summary>
    /// Edit database connection
    /// </summary>
    protected async Task EditConnectionAsync()
    {
        try
        {
            // working copy
            var editConnection = new DatabaseConnection(Asset.DatabaseConnection);
            var newConnection = Asset.DatabaseStatus == DatabaseStatus.UndefinedConnection;

            // database host
            DatabaseHost? initHost = null;
            if (newConnection)
            {
                initHost = await DialogService.ShowEnumSelect<DatabaseHost>(
                    title: Localizer.DatabaseConnectionDialogTitle,
                    message: Localizer.DatabaseHostTypeQuery,
                    reverseOrder: true);
                if (initHost == null)
                {
                    return;
                }
            }

            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(DatabaseConnectionDialog.DatabaseService), DatabaseService },
                { nameof(DatabaseConnectionDialog.Connection), editConnection },
                { nameof(DatabaseConnectionDialog.NewConnection), newConnection },
                { nameof(DatabaseConnectionDialog.Version), Asset.Parameters.Database.CurrentVersion },
                { nameof(DatabaseConnectionDialog.InitHost), initHost }
            };

            // show dialog
            var dialog = await (await DialogService.ShowAsync<DatabaseConnectionDialog>(
               title: Localizer.DatabaseConnectionDialogTitle, parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            // no changes
            if (editConnection.EqualValues(Asset.DatabaseConnection))
            {
                StatusMessageService.SetMessage(Localizer.NoEditChangesMessage);
                return;
            }

            // store user settings
            await SettingsService.SetDatabaseConnectionAsync(editConnection);

            // update asset
            Asset.DatabaseConnection.ImportValues(editConnection);

            // asset status
            await AssetService.InvalidateStatusAsync();

            // user notification
            StatusMessageService.SetMessage(Localizer.DatabaseConnectionUpdateMessage);

            // new connection
            if (newConnection)
            {
                var status = await DatabaseService.GetStatusAsync(Asset.DatabaseConnection);
                await SetupDatabaseAsync(status);
            }
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.DatabaseConnectionDialogTitle, exception);
        }
    }

    /// <summary>
    /// Setup database
    /// </summary>
    protected async Task SetupDatabaseAsync() =>
        await SetupDatabaseAsync(Asset.DatabaseStatus);

    /// <summary>
    /// Setup database
    /// </summary>
    private async Task SetupDatabaseAsync(DatabaseStatus status)
    {
        if (status.ReadyToCreate())
        {
            await CreateDatabaseAsync();
        }
        else if (status.ReadyToCreate())
        {
            await UpdateDatabaseAsync();
        }
    }

    /// <summary>
    /// Create new database
    /// </summary>
    private async Task CreateDatabaseAsync()
    {
        try
        {
            // existing database
            var version = await DatabaseService.GetCurrentVersionAsync(Asset.DatabaseConnection);
            if (version != null)
            {
                return;
            }

            // script files
            var scripts = await AssetService.GetCreateScriptsAsync();
            if (!scripts.Any())
            {
                return;
            }

            // current database status
            var databaseStatus = await DatabaseService.GetStatusAsync(Asset.DatabaseConnection);

            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(DatabaseSetupDialog.DatabaseService), DatabaseService },
                { nameof(DatabaseSetupDialog.Connection), Asset.DatabaseConnection },
                { nameof(DatabaseSetupDialog.SetupMode), DatabaseSetupMode.Create },
                { nameof(DatabaseSetupDialog.UseCollation), databaseStatus == DatabaseStatus.MissingDatabase },
                { nameof(DatabaseSetupDialog.Scripts), scripts }
            };

            // show dialog
            var dialog = await (await DialogService.ShowAsync<DatabaseSetupDialog>(
                title: Localizer.DatabaseCreateDialogTitle, parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            // check version
            version = await DatabaseService.GetCurrentVersionAsync(Asset.DatabaseConnection);
            if (!Equals(version, Asset.Parameters.Database.CurrentVersion))
            {
                await DialogService.ShowMessage(Localizer.DatabaseCreateDialogTitle,
                    Localizer.DatabaseCreateErrorMessage);
                return;
            }
            // invalidate assets status
            await AssetService.InvalidateStatusAsync();

            // user notification
            StatusMessageService.SetMessage(Localizer.DatabaseCreateSuccessMessage);
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.DatabaseConnectionDialogTitle, exception);
        }
    }

    /// <summary>
    /// Update existing database
    /// </summary>
    private async Task UpdateDatabaseAsync()
    {
        try
        {
            // existing database
            var version = await DatabaseService.GetCurrentVersionAsync(Asset.DatabaseConnection);
            if (version == null || version.IsEmpty())
            {
                return;
            }

            // script files
            var scripts = await AssetService.GetUpdateScriptsAsync(version);
            if (!scripts.Any())
            {
                return;
            }

            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(DatabaseSetupDialog.DatabaseService), DatabaseService },
                { nameof(DatabaseSetupDialog.Connection), Asset.DatabaseConnection },
                { nameof(DatabaseSetupDialog.SetupMode), DatabaseSetupMode.Update },
                { nameof(DatabaseSetupDialog.Scripts), scripts }
            };

            // show dialog
            var dialog = await (await DialogService.ShowAsync<DatabaseSetupDialog>(
                title: Localizer.DatabaseUpdateDialogTitle, parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            // check version
            version = await DatabaseService.GetCurrentVersionAsync(Asset.DatabaseConnection);
            if (!Equals(version, Asset.Parameters.Database.CurrentVersion))
            {
                await DialogService.ShowMessage(Localizer.DatabaseUpdateDialogTitle,
                    Localizer.DatabaseUpdateErrorMessage);
                return;
            }

            // invalidate assets status
            await AssetService.InvalidateStatusAsync();

            // user notification
            StatusMessageService.SetMessage(Localizer.DatabaseUpdateSuccessMessage);
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.DatabaseUpdateDialogTitle, exception);
        }
    }

    #endregion

}