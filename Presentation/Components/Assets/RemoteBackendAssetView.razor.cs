using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.WebServer;
using PayrollEngine.AdminApp.Presentation.Components.Dialogs;

namespace PayrollEngine.AdminApp.Presentation.Components.Assets;

/// <summary>
/// View for the remote backend asset
/// </summary>
public abstract class RemoteBackendAssetViewBase : ComponentBase
{
    /// <summary>
    /// Remote backend asset
    /// </summary>
    [Parameter] public RemoteBackendAsset Asset { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IStatusMessageService StatusMessageService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IAssetService AssetService { get; set; }

    private ISettingsService SettingsService => AssetService.AssetContext.SettingsService;

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
    /// Web server url href
    /// </summary>
    protected MarkupString WebServerHref
    {
        get
        {
            var serverUrl = WebServerUrl;
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                serverUrl = Localizer.UrlUndefined;
            }
            return Asset.WebServerStatus == WebServerStatus.Available ?
                // link
                MarkupTool.ToHref(serverUrl) :
                // no link
                new(serverUrl);
        }
    }

    /// <summary>
    /// Web server edit text
    /// </summary>
    protected string WebServerEditText =>
        Asset.WebServerStatus switch
        {
            WebServerStatus.UndefinedConnection => Localizer.Add,
            _ => Localizer.Edit
        };

    /// <summary>
    /// Browse the backend (open API)
    /// </summary>
    protected async Task BrowseServerAsync()
    {
        try
        {
            await Asset.BrowseAsync();
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessage(Localizer.BackendRemoteTitle, exception);
        }
    }

    /// <summary>
    /// Edit the server connection
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
                editConnection.BaseUrl = Specification.BackendDefaultBaseUrl;
                editConnection.Port = Specification.BackendDefaultPort;
            }

            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(WebServerConnectionDialog.Connection), editConnection }
            };

            // show dialog
            var dialog = await (await DialogService.ShowAsync<WebServerConnectionDialog>(
                title: Localizer.WebServerDialogTitle, parameters)).Result;
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
            await SettingsService.SetApiConnectionAsync(editConnection);

            // update asset
            Asset.WebServerConnection.ImportValues(editConnection);

            // invalidate assets status
            await AssetService.InvalidateStatusAsync();

            // user notification
            StatusMessageService.SetMessage(Localizer.WebServerConnectionUpdateMessage);
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessage(Localizer.BackendRemoteTitle, exception);
        }
    }
}