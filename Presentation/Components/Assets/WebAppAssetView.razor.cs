using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.WebServer;
using PayrollEngine.AdminApp.Presentation.Components.Dialogs;

namespace PayrollEngine.AdminApp.Presentation.Components.Assets;

/// <summary>
/// View for the web app asset
/// </summary>
public abstract class WebAppAssetViewBase : ComponentBase
{
    /// <summary>
    /// Web app asset
    /// </summary>
    [Parameter] public WebAppAsset Asset { get; set; }

    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IStatusMessageService StatusMessageService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IAssetService AssetService { get; set; }

    private ISettingsService SettingsService => AssetService.AssetContext.SettingsService;

    /// <summary>
    /// Tst if backend is running
    /// </summary>
    protected bool BackendIsRunning
    {
        get
        {
            if (AssetService.Backend.Available)
            {
                // on-prem: database must be available
                return AssetService.Backend.BackendStatus == BackendStatus.Running;
            }
            if (AssetService.RemoteBackend.Available)
            {
                // remote: web service is defined
                return AssetService.RemoteBackend.WebServerStatus == WebServerStatus.Available;
            }
            return false;
        }
    }

    /// <summary>
    /// Web server url
    /// </summary>
    protected string WebServerUrl =>
        Asset.WebServerConnection.ToUrl();

    /// <summary>
    /// Web server url style
    /// </summary>
    protected string WebServerUrlStyle =>
        Asset.WebServerStatus == WebServerStatus.Available ?
            "text-decoration: underline" : null;

    /// <summary>
    /// Web server href
    /// </summary>
    protected MarkupString WebServerHref =>
        MarkupTool.ToHref(WebServerUrl);

    /// <summary>
    /// Web server edit text
    /// </summary>
    protected string WebServerEditText =>
        Asset.WebServerStatus switch
        {
            WebServerStatus.UndefinedConnection => Localizer.Add,
            _ => Localizer.EditDatabase
        };

    /// <summary>
    /// Brows web server: web app login 
    /// </summary>
    protected async Task BrowseServerAsync()
    {
        try
        {
            await Asset.BrowseAsync();
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessageBox(Localizer.WebAppTitle, exception);
        }
    }

    /// <summary>
    /// Start the web app server
    /// </summary>
    protected async Task StartServerAsync()
    {
        try
        {
            if (!await CheckCertificate())
            {
                return;
            }
            await Asset.StartAsync();
            await AssetService.InvalidateStatusAsync();
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessageBox(Localizer.WebAppTitle, exception);
        }
    }

    /// <summary>
    /// Edit web server connection
    /// </summary>
    protected async Task EditServerAsync()
    {
        try
        {
            // working copy
            var editConnection = new WebServerConnection(Asset.WebServerConnection);

            // init
            if (editConnection.IsEmpty())
            {
                editConnection.BaseUrl = Specification.WebAppDefaultBaseUrl;
                editConnection.Port = Specification.WebAppDefaultPort;
            }

            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(WebServerConnectionDialog.Connection), editConnection },
                // ignore non-url fields
                { nameof(WebServerConnectionDialog.UrlOnly), true }
            };

            // show dialog
            var dialog = await (await DialogService.ShowAsync<WebServerConnectionDialog>(
                title: Localizer.DatabaseConnectionDialogTitle, parameters)).Result;
            if (dialog == null || dialog.Canceled)
            {
                return;
            }

            // no changes
            if (editConnection.EqualValues(Asset.WebServerConnection))
            {
                StatusMessageService.SetMessage(Localizer.NoEditChangesMessage);
                return;
            }

            // store user settings
            await SettingsService.SetWebAppConnectionAsync(editConnection);

            // update asset
            Asset.WebServerConnection.ImportValues(editConnection);

            // invalidate assets status
            await AssetService.InvalidateStatusAsync();

            // user notification
            StatusMessageService.SetMessage(Localizer.WebServerConnectionUpdateMessage);
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessageBox(Localizer.DatabaseConnectionDialogTitle, exception);
        }
    }

    /// <summary>
    /// Check for local https dev certificate
    /// </summary>
    private async Task<bool> CheckCertificate()
    {
        // check for .net dev https certificate
        if (!Asset.WebServerConnection.IsLocalSecureConnection() ||
            OperatingSystem.HasLocalSecureDevCertificate())
        {
            // available
            return true;
        }

        // confirm certificate installation
        var dialog = await DialogService.ShowMessageBox(
            title: Localizer.WebAppTitle,
            markupMessage: Localizer.MissingDotNetDevCertificate,
            cancelText: Localizer.Cancel);
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
            dialog = await DialogService.ShowMessageBox(
                title: Localizer.WebAppTitle,
                markupMessage: Localizer.DotNetDevCertificateSetupFailed,
                cancelText: Localizer.Cancel);
            return dialog == true;
        }

        // successful installed
        return true;
    }
}