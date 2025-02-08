using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Presentation.Components.Dialogs;

namespace PayrollEngine.AdminApp.Presentation.Components.Assets;

/// <summary>
/// View for the console asset
/// </summary>
public abstract class ConsoleAssetViewBase : ComponentBase
{
    /// <summary>
    /// Console asset
    /// </summary>
    [Parameter] public ConsoleAsset Asset { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IDialogService DialogService { get; set; }

    /// <summary>
    /// Test for file type register
    /// </summary>
    protected bool CanRegisterFileType { get; private set; }

    /// <summary>
    /// Test admin for file type register
    /// </summary>
    protected bool AdminCanRegisterFileType { get; private set; }

    /// <summary>
    /// Register the file types (windows only)
    /// </summary>
    protected async Task RegisterFileTypeAsync()
    {
        try
        {
            // dialog parameters
            var parameters = new DialogParameters
            {
                { nameof(FileTypeRegisterDialog.FileTypeName), Asset.Parameters.FileTypeName },
                { nameof(FileTypeRegisterDialog.FileTypeExtension), Asset.Parameters.FileTypeExtension },
                { nameof(FileTypeRegisterDialog.Executable), Asset.Parameters.Executable }
            };

            // show dialog
            await DialogService.ShowAsync<FileTypeRegisterDialog>(
                title: Localizer.FileTypeRegisterTitle, parameters);
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessage(Localizer.ConsoleTitle, exception);
        }
    }

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        CanRegisterFileType = OperatingSystem.IsAdministrator();
        AdminCanRegisterFileType = OperatingSystem.IsWindows();
        base.OnInitialized();
    }
}