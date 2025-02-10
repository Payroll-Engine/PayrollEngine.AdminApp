using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Presentation.Components.Dialogs;

/// <summary>
/// Database parameter dialog
/// </summary>
public abstract class DatabaseParameterDialogBase : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    /// <summary>
    /// Database connection parameter
    /// </summary>
    [Parameter] public ConnectionParameter Parameter { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IDialogService DialogService { get; set; }

    /// <summary>
    /// Edit form
    /// </summary>
    protected MudForm Form { get; set; }

    /// <summary>
    /// Test for valid form
    /// </summary>
    protected bool IsValid { get; set; }

    /// <summary>
    /// The editing database parameter
    /// </summary>
    protected ConnectionParameter EditParameter { get; } = new();

    /// <summary>
    /// Cancel dialog
    /// </summary>
    protected void Cancel() => MudDialog.Cancel();

    /// <summary>
    /// Submit database parameter
    /// </summary>
    protected async Task Submit()
    {
        try
        {
            // form validation
            Form.ResetValidation();
            await Form.Validate();

            if (!Form.IsValid)
            {
                return;
            }

            // adapt edit values
            Parameter.ImportValues(EditParameter);
            MudDialog.CloseSuccess();
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.DatabaseConnectionDialogTitle, exception);
            MudDialog.CloseFailure();
        }
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        // init
        if (Parameter != null)
        {
            EditParameter.ImportValues(Parameter);
        }
        await base.OnInitializedAsync();
    }
}