using System;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Webserver;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Presentation.Components.Pages;

/// <summary>
/// Main app page
/// </summary>
public class MainPage : ComponentBase, IDisposable
{
    /// <summary>
    /// Localizer
    /// </summary>
    [Inject]
    protected Localizer Localizer { get; set; }
    /// <summary>
    /// Asset service
    /// </summary>
    [Inject]
    protected IAssetService AssetService { get; set; }
    [Inject]
    private IStatusMessageService StatusMessageService { get; set; }
    [Inject] 
    private IStatusUpdateService StatusUpdateService { get; set; }
    [Inject]
    private IConfigurationRoot Configuration { get; set; }

    #region Backend

    private BackendStatus BackendStatus => AssetService.Backend.BackendStatus;

    /// <summary>
    /// Backend asset available
    /// </summary>
    protected bool BackendAvailable =>
        AssetService.Backend.Available && BackendStatus > BackendStatus.NotAvailable;

    /// <summary>
    /// Backend webserver status
    /// </summary>
    protected WebserverStatus BackendServerStatus => AssetService.Backend.WebserverStatus;

    /// <summary>
    /// Backend database status
    /// </summary>
    protected DatabaseStatus BackendDatabaseStatus => AssetService.Backend.DatabaseStatus;

    private bool BackendForClientAvailable
    {
        get
        {
            if (AssetService.Backend.Available)
            {
                // on-prem: database must be available
                return AssetService.Backend.BackendStatus > BackendStatus.WebserverUndefined;
            }
            if (AssetService.RemoteBackend.Available)
            {
                // remote: web service is defined
                return AssetService.RemoteBackend.WebserverStatus > WebserverStatus.UndefinedConnection;
            }
            return false;
        }
    }

    #endregion

    #region Web App

    private WebAppStatus WebAppStatus => AssetService.WebApp.WebAppStatus;

    /// <summary>
    /// Web app webserver status
    /// </summary>
    protected WebserverStatus WebAppServerStatus => AssetService.WebApp.WebserverStatus;

    /// <summary>
    /// Web app asset available
    /// </summary>
    protected bool WebAppAvailable =>
        AssetService.WebApp.Available &&
        BackendForClientAvailable && WebAppStatus > WebAppStatus.NotAvailable;

    #endregion

    #region Assets

    /// <summary>
    /// Test for any asset
    /// </summary>
    protected bool HasAnyAsset =>
        AssetService.Backend.Available ||
        AssetService.WebApp.Available ||
        AssetService.Console.Available ||
        AssetService.Tests.Available ||
        AssetService.Examples.Available;

    /// <summary>
    /// Console asset available
    /// </summary>
    protected bool ConsoleAvailable => AssetService.Console.Available;

    /// <summary>
    /// Tests asset available
    /// </summary>
    protected bool TestsAvailable => AssetService.Tests.Available;

    /// <summary>
    /// Examples asset available
    /// </summary>
    protected bool ExamplesAvailable => AssetService.Examples.Available;

    #endregion

    #region Status

    /// <summary>
    /// Last status update date
    /// </summary>
    protected DateTime StatusLastUpdated { get; private set; } = DateTime.Now;

    /// <summary>
    /// Status updating indicator
    /// </summary>
    protected bool StatusUpdating => StatusUpdateService.Updating;

    /// <summary>
    /// Status icon
    /// </summary>
    protected string StatusIcon { get; private set; }

    /// <summary>
    /// Status markup message
    /// </summary>
    protected MarkupString StatusMessage { get; private set; }

    /// <summary>
    /// Status type
    /// </summary>
    protected StatusMessageType StatusType { get; private set; }

    /// <summary>
    /// Test for status message
    /// </summary>
    protected bool HasStatusMessage =>
        !string.IsNullOrWhiteSpace(StatusMessage.Value);

    private void UpdateStatusMessage()
    {
        StatusMessage = new(StatusMessageService.Message);
        StatusType = StatusMessageService.MessageType;
        StatusIcon = StatusType == StatusMessageType.Error ?
                Icons.Material.Outlined.Error :
                Icons.Material.Outlined.Info;

        StateHasChanged();
    }

    #endregion

    #region Timer

    private Timer Timer { get; } = new(TimeSpan.FromSeconds(Specification.AppRefreshDefaultTimeout));

    /// <summary>
    /// Get the refresh help
    /// </summary>
    protected string RefreshHelp => ActiveTimer
        ? Localizer.AutoRefreshAppStatusHelp((int)(Timer.Interval / 1000))
        : Localizer.RefreshAppStatusHelp;

    /// <summary>
    /// Test for active timer
    /// </summary>
    protected bool ActiveTimer => Timer.Enabled;

    /// <summary>
    /// Initialize the timer
    /// </summary>
    private void InitTimer()
    {
        // timeout setting
        var timeout = Specification.AppRefreshDefaultTimeout;
        var timeoutText = Configuration[Specification.AutoRefreshTimeoutConfig];
        if (!string.IsNullOrWhiteSpace(timeoutText) &&
            int.TryParse(timeoutText, out var userTimeout))
        {
            // disabled timer
            if (userTimeout <= 0)
            {
                return;
            }
            // user timeout
            if (userTimeout >= Specification.AppRefreshMinTimeout &&
                userTimeout <= Specification.AppRefreshMaxTimeout)
            {
                timeout = userTimeout;
            }
        }

        // timer interval from seconds to milliseconds
        Timer.Interval = timeout * 1000;
        Timer.Elapsed += TimerElapsedHandler;
        // enable refresh loop
        Timer.AutoReset = true;
        Timer.Enabled = true;
    }

    private void TimerElapsedHandler(object source, ElapsedEventArgs e)
    {
        if (StatusUpdating)
        {
            return;
        }
        InvokeAsync(UpdateStatusAsync);
    }

    #endregion

    #region Lifecycle

    /// <summary>
    /// Update assets status
    /// </summary>
    protected async Task UpdateStatusAsync()
    {
        if (StatusUpdating)
        {
            return;
        }

        // show progress indicator
        StatusUpdateService.BeginUpdate();

        StateHasChanged();
        try
        {
            await Task.Run(AssetService.UpdateStatusAsync);
            StatusLastUpdated = DateTime.Now;
        }
        catch (Exception exception)
        {
            StatusMessageService.SetError(exception);
        }
        finally
        {
            // hide progress indicator
            StatusUpdateService.EndUpdate();
            StateHasChanged();
        }
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        // status message
        StatusMessageService.MessageChanged += StatusMessageChangedHandler;
        AssetService.StatusInvalidated += AssetStatusInvalidatedHandler;
        StatusMessageService.SetMessage(Localizer.AppLoadingMessage);

        // timer
        InitTimer();

        await base.OnInitializedAsync();
    }

    /// <summary>
    /// Test for loaded page
    /// </summary>
    protected bool Loaded { get; private set; }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        var updateStatus = firstRender || !AssetService.ValidStatus;

        // update status
        if (updateStatus)
        {
            await UpdateStatusAsync();
        }

        // first render only
        if (firstRender)
        {
            StatusMessageService.SetMessage(Localizer.AppWelcomeMessage);
            Loaded = true;
            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private void StatusMessageChangedHandler(object sender, EventArgs e) =>
        UpdateStatusMessage();

    private void AssetStatusInvalidatedHandler(object sender, EventArgs e)
    {
        if (!StatusUpdating)
        {
            StateHasChanged();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        AssetService.StatusInvalidated -= AssetStatusInvalidatedHandler;
        StatusMessageService.MessageChanged -= StatusMessageChangedHandler;
        Timer.Dispose();
    }

    #endregion

}