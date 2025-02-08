using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace PayrollEngine.AdminApp.Presentation.Components.Dialogs;

/// <summary>
/// File type register dialog
/// </summary>
public abstract class FileTypeRegisterDialogBase : ComponentBase
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    /// <summary>
    /// File type name
    /// </summary>
    [Parameter] public string FileTypeName { get; set; }

    /// <summary>
    /// File type extension
    /// </summary>
    [Parameter] public string FileTypeExtension { get; set; }

    /// <summary>
    /// Executable file name
    /// </summary>
    [Parameter] public string Executable { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
    [Inject] private IStatusMessageService StatusMessageService { get; set; }
    [Inject] private IDialogService DialogService { get; set; }

    /// <summary>
    /// Cancel dialog
    /// </summary>
    protected void Cancel() => MudDialog.Cancel();

    /// <summary>
    /// Register file type
    /// </summary>
    protected async Task Register()
    {
        try
        {
            // confirm file type registration
            var dialog = await DialogService.ShowConfirm(
                title: Localizer.FileTypeRegisterTitle,
                message: Localizer.FileTypeRegisterQuery(FileTypeExtension));
            if (dialog != true)
            {
                return;
            }

            var result = OperatingSystem.RegisterFileType(FileTypeName, FileTypeExtension, Executable);
            if (result != 0)
            {
                await DialogService.ShowMessage(Localizer.FileTypeRegisterTitle, Localizer.FileTypeRegisterError);
                return;
            }

            // user notification
            StatusMessageService.SetMessage(Localizer.FileTypeRegisterSuccess);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessage(Localizer.FileTypeRegisterTitle, exception);
            MudDialog.Close(DialogResult.Ok(false));
        }
    }

    /// <summary>
    /// Unregister file type
    /// </summary>
    protected async Task Unregister()
    {
        try
        {
            // confirm file type registration
            var dialog = await DialogService.ShowConfirm(
                title: Localizer.FileTypeUnregisterTitle,
                message: Localizer.FileTypeUnregisterQuery(FileTypeExtension));
            if (dialog != true)
            {
                return;
            }

            var result = OperatingSystem.UnregisterFileType(FileTypeName, FileTypeExtension);
            if (result != 0)
            {
                await DialogService.ShowMessage(Localizer.FileTypeRegisterTitle, Localizer.FileTypeUnregisterError);
                return;
            }

            // user notification
            StatusMessageService.SetMessage(Localizer.FileTypeUnregisterSuccess);
            MudDialog.Close(DialogResult.Ok(true));
        }
        catch (Exception exception)
        {
            await DialogService.ShowMessage(Localizer.FileTypeRegisterTitle, exception);
            MudDialog.Close(DialogResult.Ok(false));
        }
    }
}