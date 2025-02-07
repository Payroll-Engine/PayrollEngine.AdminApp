using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Presentation.Components.Dialogs;

/// <summary>
/// Database setup dialog
/// </summary>
public abstract class DatabaseSetupDialogBase : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    /// <summary>
    /// Database service
    /// </summary>
    [Parameter] public IDatabaseService DatabaseService { get; set; }

    /// <summary>
    /// Database connection
    /// </summary>
    [Parameter] public DatabaseConnection Connection { get; set; }
    
    /// <summary>
    /// Database setup mode
    /// </summary>
    [Parameter] public DatabaseSetupMode SetupMode { get; set; }
    
    /// <summary>
    /// Database collation
    /// </summary>
    [Parameter] public string Collation { get; set; }
    
    /// <summary>
    /// Database scripts
    /// </summary>
    [Parameter] public List<string> Scripts { get; set; }

    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IErrorService ErrorService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }

    /// <summary>
    /// Current status message
    /// </summary>
    protected string StatusMessage { get; private set; }

    /// <summary>
    /// Test for executing setup
    /// </summary>
    protected bool SetupExecute { get; private set; }

    protected bool UseCollation =>
        SetupMode == DatabaseSetupMode.Create && DatabaseStatus == DatabaseStatus.MissingDatabase;

    private DatabaseStatus DatabaseStatus { get; set; }

    private string DialogTitle =>
        SetupMode == DatabaseSetupMode.Create
            ? Localizer.DatabaseCreateDialogTitle
            : Localizer.DatabaseUpdateDialogTitle;

    private string SuccessMessage =>
        SetupMode == DatabaseSetupMode.Create
            ? Localizer.DatabaseCreateSuccessMessage
            : Localizer.DatabaseUpdateSuccessMessage;

    /// <summary>
    /// Cancel dialog
    /// </summary>
    protected void Cancel() => MudDialog.Cancel();

    /// <summary>
    /// Setup database
    /// </summary>
    /// <returns></returns>
    protected async Task Submit()
    {
        // show progress indicator
        SetupExecute = true;
        StatusMessage = Localizer.DatabaseInstallInfo;
        StateHasChanged();
        try
        {
            await InvokeAsync(Setup);
        }
        finally
        {
            // hide progress indicator
            SetupExecute = false;
            StateHasChanged();
        }
    }

    private async Task Setup()
    {
        try
        {
            // error watch register
            var watchName = nameof(DatabaseSetupDialog);
            ErrorService.AddWatch(watchName);

            // create database
            if (SetupMode == DatabaseSetupMode.Create && DatabaseStatus == DatabaseStatus.MissingDatabase)
            {
                StatusMessage = Localizer.DatabaseInstallInfo;
                StateHasChanged();
                var databaseResult = await DatabaseService.CreateDatabaseAsync(Connection, Collation);
                if (databaseResult != 0)
                {
                    await DialogService.ShowMessageBox(DialogTitle,
                        Localizer.DatabaseCreateError(ErrorService.GetErrorHistory(watchName)));
                    MudDialog.Close(DialogResult.Ok(false));
                    return;
                }
            }

            // execute scripts
            StatusMessage = Localizer.ScriptInstallInfo;
            StateHasChanged();
            var scriptError = false;
            foreach (var script in Scripts)
            {
                var rows = await DatabaseService.ExecuteScriptAsync(Connection, script);
                if (rows < 0)
                {
                    scriptError = true;
                    break;
                }
            }

            // hide progress indicator
            SetupExecute = false;
            StateHasChanged();

            // script error
            // watch unregister
            var errors = ErrorService.GetErrorHistory(watchName,
                queryMode: ErrorWatchQueryMode.RemoveWatch);
            if (scriptError && !string.IsNullOrWhiteSpace(errors))
            {
                await DialogService.ShowMessageBox(DialogTitle,
                    Localizer.DatabaseSetupError(errors));
                MudDialog.Close(DialogResult.Ok(false));
                return;
            }

            // success
            await DialogService.ShowMessageBox(DialogTitle, SuccessMessage);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessageBox(DialogTitle, exception);
            MudDialog.Close(DialogResult.Ok(false));
        }
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            DatabaseStatus = await DatabaseService.GetStatusAsync(Connection);
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
}