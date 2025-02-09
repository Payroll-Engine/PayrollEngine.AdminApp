using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Webserver;

namespace PayrollEngine.AdminApp.Presentation.Components.Dialogs;

/// <summary>
/// Webserver connection dialog
/// </summary>
public abstract class WebserverConnectionDialogBase : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    /// <summary>
    /// Webserver connection
    /// </summary>
    [Parameter] public WebserverConnection Connection { get; set; }

    /// <summary>
    /// Only url edit
    /// </summary>
    [Parameter] public bool UrlOnly { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IDialogService DialogService { get; set; }

    /// <summary>
    /// Api key input type
    /// </summary>
    protected InputType ApiKeyInputType { get; private set; } = InputType.Password;

    /// <summary>
    /// Api key input icon
    /// </summary>
    protected string ApiKeyInputIcon { get; private set; } = Icons.Material.Filled.Visibility;

    /// <summary>
    /// Api key visibility
    /// </summary>
    private bool ApiKeyVisible { get; set; }

    /// <summary>
    /// Editor form
    /// </summary>
    protected MudForm Form { get; set; }

    /// <summary>
    /// Test for valid form
    /// </summary>
    protected bool IsValid { get; set; }

    /// <summary>
    /// The editing connection
    /// </summary>
    protected WebserverConnection EditConnection { get; } = new();

    /// <summary>
    /// Connection timeout
    /// </summary>
    protected int Timeout
    {
        get => (int)EditConnection.Timeout.TotalSeconds;
        set => EditConnection.Timeout = TimeSpan.FromSeconds(value);
    }

    /// <summary>
    /// Cancel dialog
    /// </summary>
    protected void Cancel() => MudDialog.Cancel();

    /// <summary>
    /// Submit webserver connection
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

            // adapt edit values
            Connection.ImportValues(EditConnection);

            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception exception)
        {
            await DialogService.ShowErrorMessage(Localizer.DatabaseConnectionDialogTitle, exception);
            MudDialog.Close(DialogResult.Ok(false));
        }
    }

    /// <summary>
    /// Toggle the api key visibility
    /// </summary>
    protected void ToggleApiKeyVisibility()
    {
        if (ApiKeyVisible)
        {
            ApiKeyVisible = false;
            ApiKeyInputIcon = Icons.Material.Filled.Visibility;
            ApiKeyInputType = InputType.Password;
        }
        else
        {
            ApiKeyVisible = true;
            ApiKeyInputIcon = Icons.Material.Filled.VisibilityOff;
            ApiKeyInputType = InputType.Text;
        }
    }

    /// <inheritdoc />
    protected override async Task OnInitializedAsync()
    {
        // init
        if (Connection != null)
        {
            EditConnection.ImportValues(Connection);
        }
        await base.OnInitializedAsync();
    }
}