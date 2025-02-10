using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
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
    /// Database collation mode
    /// </summary>
    [Parameter] public bool UseCollation { get; set; }

    /// <summary>
    /// Database scripts
    /// </summary>
    [Parameter] public List<string> Scripts { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IErrorService ErrorService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }
    [Inject] private IConfigurationRoot Configuration { get; set; }

    private const int TestCreateRetryCount = 100;
    private const int TestCreateRetryDelay = 200; // milliseconds

    /// <summary>
    /// Database collation
    /// </summary>
    protected string Collation { get; set; }

    /// <summary>
    /// Current status message
    /// </summary>
    protected string StatusMessage { get; private set; }

    /// <summary>
    /// Test for executing setup
    /// </summary>
    protected bool SetupExecute { get; private set; }

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
    protected async Task Submit()
    {
        if (SetupExecute)
        {
            return;
        }

        // show progress indicator
        SetupExecute = true;
        StatusMessage = Localizer.DatabaseInstallInfo;
        StateHasChanged();

        try
        {
            // error handling
            ErrorService.Reset();

            // database setup
            if (SetupMode == DatabaseSetupMode.Create)
            {
                var databaseStatus = await DatabaseService.GetStatusAsync(Connection);
                if (databaseStatus == DatabaseStatus.MissingDatabase)
                {
                    // create database with error tracking
                    var createResult = await Task.Run(() =>
                        DatabaseService.CreateDatabaseAsync(Connection, Collation, ErrorService));
                    if (createResult != true)
                    {
                        await DialogService.ShowMessage(DialogTitle,
                            ErrorService.RetrieveErrors() ?? Localizer.DatabaseCreateErrorMessage);
                        return;
                    }

                    // test created database
                    var count = 0;
                    var status = await DatabaseService.GetStatusAsync(Connection);
                    while (status != DatabaseStatus.EmptyDatabase && count < TestCreateRetryCount)
                    {
                        // retry with some delay
                        await Task.Delay(TestCreateRetryDelay);
                        status = await DatabaseService.GetStatusAsync(Connection);
                        count++;
                    }
                    if (status != DatabaseStatus.EmptyDatabase)
                    {
                        await DialogService.ShowMessage(DialogTitle, Localizer.DatabaseCreateErrorMessage);
                        return;
                    }
                }
            }

            // execute scripts
            StatusMessage = Localizer.ScriptInstallInfo;
            StateHasChanged();

            // scripts
            var scriptError = false;
            foreach (var script in Scripts)
            {
                // execute script with error tracking
                var rows = await Task.Run(() => DatabaseService.ExecuteScriptAsync(Connection, script, ErrorService));
                if (rows == 0)
                {
                    scriptError = true;
                    break;
                }
            }

            // hide progress indicator
            SetupExecute = false;
            StateHasChanged();

            // script error
            if (scriptError)
            {
                StatusMessage = ErrorService.RetrieveErrors() ?? Localizer.DatabaseCreateErrorMessage;
                return;
            }

            // close dialog
            MudDialog.CloseSuccess();

            // user confirmation
            await InvokeAsync(SetupCompletedAsync);
        }
        catch (Exception exception)
        {
            StatusMessage = exception.GetBaseException().Message;
        }
        finally
        {
            // hide progress indicator
            SetupExecute = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Setup completed message
    /// </summary>
    private async Task SetupCompletedAsync()
    {
        await DialogService.ShowMessage(DialogTitle, SuccessMessage);
    }

    private void InitCollation()
    {
        if (UseCollation)
        {
            Collation = Configuration["DatabaseCollation"] ?? Specification.DefaultDatabaseCollation;
        }
    }

    /// <inheritdoc />
    protected override void OnParametersSet()
    {
        InitCollation();
        base.OnParametersSet();
    }
}