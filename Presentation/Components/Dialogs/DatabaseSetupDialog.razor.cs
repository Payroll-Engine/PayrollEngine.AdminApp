﻿using System;
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
    /// Database scripts
    /// </summary>
    [Parameter] public List<string> Scripts { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
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
                var databaseStatus = await Task.Run(() => DatabaseService.GetStatusAsync(Connection));
                if (databaseStatus == DatabaseStatus.MissingDatabase)
                {
                    // create database
                    var createResult = await Task.Run(() => DatabaseService.CreateDatabaseAsync(Connection));
                    if (createResult != 0)
                    {
                        // create database error
                        return;
                    }
                }

                // check for empty database
                var emptyDatabase = await Task.Run(() => DatabaseService.IsEmptyDatabaseAsync(Connection));
                if (emptyDatabase != true)
                {
                    await DialogService.ShowMessage(DialogTitle, Localizer.DatabaseNotEmptyMessage);
                    return;
                }
            }

            // execute scripts
            StatusMessage = Localizer.ScriptInstallInfo;
            StateHasChanged();

            // script install
            var scriptError = false;
            foreach (var script in Scripts)
            {
                var rows = await Task.Run(() => DatabaseService.ExecuteScriptAsync(Connection, script));
                if (rows == 0)
                {
                    scriptError = true;
                    break;
                }
            }

            // hide progress indicator
            SetupExecute = false;
            StateHasChanged();

            // error
            if (scriptError)
            {
                StatusMessage = ErrorService.RetrieveErrors();
            }

            // close dialog
            MudDialog.Close(DialogResult.Ok(true));

            // user confirmation
            await InvokeAsync(SetupCompleted);
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
    private async Task SetupCompleted()
    {
        await DialogService.ShowMessage(DialogTitle, SuccessMessage);
    }
}