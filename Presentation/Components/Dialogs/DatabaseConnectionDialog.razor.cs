using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Presentation.Components.Dialogs;

/// <summary>
/// Database connection dialog
/// </summary>
public abstract class DatabaseConnectionDialogBase : ComponentBase
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
    /// Setup new connection
    /// </summary>
    [Parameter] public bool NewConnection { get; set; }

    /// <summary>
    /// Database version
    /// </summary>
    [Parameter] public Version Version { get; set; }

    /// <summary>
    /// Database host (init only)
    /// </summary>
    [Parameter] public DatabaseHost? InitHost { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IErrorService ErrorService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }

    private bool PasswordVisible { get; set; }

    /// <summary>
    /// Editor form
    /// </summary>
    protected MudForm Form { get; set; }

    /// <summary>
    /// The editing database connection
    /// </summary>
    protected DatabaseConnection EditConnection { get; } = new();

    /// <summary>
    /// Database status
    /// </summary>
    protected DatabaseStatus DatabaseStatus { get; private set; }

    /// <summary>
    /// Password input type
    /// </summary>
    protected InputType PasswordInputType { get; private set; } = InputType.Password;

    /// <summary>
    /// Password input icon
    /// </summary>
    protected string PasswordInputIcon { get; private set; } = Icons.Material.Filled.Visibility;

    /// <summary>
    /// Test for valid database connection
    /// </summary>
    protected bool IsValid { get; set; }

    /// <summary>
    /// Submit button text
    /// </summary>
    protected string SubmitText { get; private set; }

    #region Parameter

    /// <summary>
    /// Add database connection parameter
    /// </summary>
    protected async Task AddParameterAsync()
    {
        // dialog parameters
        ConnectionParameter parameter = new();
        var parameters = new DialogParameters
        {
            { nameof(ConnectionParameterDialog.Parameter), parameter }
        };

        // show dialog
        var dialog = await (await DialogService.ShowAsync<ConnectionParameterDialog>(
            title: Localizer.ConnectionParameterDialogTitle, parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // existing
        if (EditConnection.CustomParameters.Contains(parameter.Name))
        {
            await DialogService.ShowMessage(
                Localizer.ConnectionParameterDialogTitle,
                Localizer.InvalidConnectionParameter(parameter.Name));
            return;
        }

        // add parameter
        EditConnection.CustomParameters.Set(parameter);
    }

    /// <summary>
    /// Edit database connection parameter
    /// </summary>
    protected async Task EditParameterAsync(ConnectionParameter parameter)
    {
        ConnectionParameter editParameter = new(parameter);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(ConnectionParameterDialog.Parameter), editParameter }
        };

        // show dialog
        var dialog = await (await DialogService.ShowAsync<ConnectionParameterDialog>(
            title: Localizer.ConnectionParameterDialogTitle, parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // update parameter
        parameter.ImportValues(editParameter);
    }

    /// <summary>
    /// Remove database connection parameter
    /// </summary>
    protected async Task RemoveParameterAsync(ConnectionParameter parameter)
    {
        // confirmation
        var dialog = await DialogService.ShowConfirm(
            Localizer.ConnectionParameterDialogTitle,
            Localizer.RemoveParameterQuery(parameter.Name));
        if (dialog == null || dialog == false)
        {
            return;
        }

        // remove parameter
        EditConnection.CustomParameters.Remove(parameter);
    }

    #endregion

    #region Actions

    /// <summary>
    /// Cancel dialog
    /// </summary>
    protected void Cancel() => MudDialog.Cancel();

    /// <summary>
    /// Submit database connection
    /// </summary>
    protected async Task Submit()
    {
        try
        {
            // form validation
            Form.ResetValidation();
            await Form.Validate();
            if (!Form.IsValid || !EditConnection.HasRequiredValues())
            {
                return;
            }

            // nes connection
            if (NewConnection)
            {
                var status = await GetDatabaseStatusAsync();
                // database not available
                if (!status.PendingChange() && status != DatabaseStatus.Available)
                {
                    await DialogService.ShowMessage(
                            Localizer.DatabaseConnectionDialogTitle,
                            Localizer.InvalidDatabaseConnection);
                    return;
                }
            }

            // adapt edit values
            Connection.ImportValues(EditConnection);

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessage(Localizer.DatabaseConnectionDialogTitle, exception);
            MudDialog.Close(DialogResult.Ok(false));
        }
    }

    /// <summary>
    /// Toggle the password visibility
    /// </summary>
    protected void TogglePasswordVisibility()
    {
        if (PasswordVisible)
        {
            PasswordVisible = false;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInputType = InputType.Password;
        }
        else
        {
            PasswordVisible = true;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInputType = InputType.Text;
        }
    }

    #endregion

    #region Lifecycle

    /// <summary>
    /// Status markup message
    /// </summary>
    protected string StatusMessage { get; private set; }

    /// <summary>
    /// Test for status message
    /// </summary>
    protected bool HasStatusMessage =>
        !string.IsNullOrWhiteSpace(StatusMessage);

    /// <summary>
    /// Check for status update
    /// </summary>
    protected bool StatusUpdate { get; private set; }

    /// <summary>
    /// Updated database connection status
    /// </summary>
    protected async Task UpdateStatusAsync()
    {
        if (!EditConnection.HasRequiredValues() || StatusUpdate)
        {
            return;
        }

        // show progress indicator
        StatusUpdate = true;
        StateHasChanged();

        try
        {
            // reset errors
            ErrorService.Reset();

            // refresh database status
            DatabaseStatus = await GetDatabaseStatusAsync(ErrorService);

            // control
            UpdateSubmitText();

            // errors
            StatusMessage = ErrorService.RetrieveErrors();
        }
        catch (AdminException exception)
        {
            await DialogService.ShowMessage(Localizer.DatabaseConnectionDialogTitle, exception);
        }
        finally
        {
            // hide progress indicator
            StatusUpdate = false;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Get the database status
    /// </summary>
    private async Task<DatabaseStatus> GetDatabaseStatusAsync(IErrorService errorService = null) =>
        await DatabaseService.GetStatusAsync(EditConnection, Version, errorService);

    /// <summary>
    /// Update submit text
    /// </summary>
    private void UpdateSubmitText()
    {
        var text = Localizer.Ok;
        if (NewConnection && DatabaseStatus.PendingChange())
        {
            text = Localizer.Continue;
        }
        SubmitText = text;
    }

    /// <summary>
    /// Initialize the database connection
    /// </summary>
    private async Task InitConnectionAsync()
    {
        // init database status
        DatabaseStatus = DatabaseStatus.UndefinedConnection;

        // copy edit values
        EditConnection.ImportValues(Connection);

        // initialize connection by host
        if (EditConnection.IsEmpty() && InitHost.HasValue)
        {
            EditConnection.Initialize(InitHost.Value);
        }

        if (!EditConnection.HasRequiredValues())
        {
            return;
        }

        // refresh database status
        DatabaseStatus = await GetDatabaseStatusAsync();
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        await InitConnectionAsync();
        UpdateSubmitText();

        await base.OnInitializedAsync();
    }

    #endregion

}