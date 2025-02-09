using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.Webserver;
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
    /// Webserver url href
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
            await DialogService.ShowErrorMessage(Localizer.BackendRemoteTitle, exception);
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
            var editConnection = new WebserverConnection(Asset.WebserverConnection);

            // init
            if (editConnection.IsEmpty())
            {
                editConnection.BaseUrl = Specification.BackendDefaultBaseUrl;
                editConnection.Port = Specification.BackendDefaultPort;
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

            // update asset
            Asset.WebserverConnection.ImportValues(editConnection);

            // invalidate assets status
            await AssetService.InvalidateStatusAsync();

            // user notification
            StatusMessageService.SetMessage(Localizer.WebserverConnectionUpdateMessage);
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.BackendRemoteTitle, exception);
        }
    }
}