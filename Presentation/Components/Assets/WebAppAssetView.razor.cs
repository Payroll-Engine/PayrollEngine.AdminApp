using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.Webserver;
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

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IStatusMessageService StatusMessageService { get; set; }
    [Inject] private IStatusUpdateService StatusUpdateService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IAssetService AssetService { get; set; }

    private ISettingsService SettingsService => AssetService.AssetContext.SettingsService;

    /// <summary>
    /// Status updating indicator
    /// </summary>
    protected bool StatusUpdating => StatusUpdateService.Updating;

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
                return AssetService.RemoteBackend.WebserverStatus == WebserverStatus.Available;
            }
            return false;
        }
    }

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
    /// Brows webserver: web app login 
    /// </summary>
    protected async Task BrowseServerAsync()
    {
        try
        {
            await Asset.BrowseAsync();
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.WebAppTitle, exception);
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
            await Asset.StartAsync(AssetService.AssetContext);
            await AssetService.InvalidateStatusAsync();
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.WebAppTitle, exception);
        }
    }

    /// <summary>
    /// Edit webserver connection
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
                editConnection.BaseUrl = Specification.WebAppDefaultBaseUrl;
                editConnection.Port = Specification.WebAppDefaultPort;
            }

            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(WebserverConnectionDialog.Connection), editConnection },
                // ignore non-url fields
                { nameof(WebserverConnectionDialog.UrlOnly), true }
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
            await SettingsService.SetWebAppConnectionAsync(editConnection);

            // update asset
            Asset.WebserverConnection.ImportValues(editConnection);

            // invalidate assets status
            await AssetService.InvalidateStatusAsync();

            // user notification
            StatusMessageService.SetMessage(Localizer.WebserverConnectionUpdateMessage);
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.WebserverDialogTitle, exception);
        }
    }

    /// <summary>
    /// Check for local https dev certificate
    /// </summary>
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
            title: Localizer.WebAppTitle,
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
                title: Localizer.WebAppTitle,
                markupMessage: Localizer.DotNetDevCertificateSetupFailed);
            return dialog == true;
        }

        // successful installed
        return true;
    }
}