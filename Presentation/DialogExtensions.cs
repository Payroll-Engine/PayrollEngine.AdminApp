using MudBlazor;

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Extension methods for dialogs
/// </summary>
public static class DialogExtensions
{
    /// <summary>
    /// Close dialog with success
    /// </summary>
    /// <param name="dialog">Dialog to close</param>
    public static void CloseSuccess(this IMudDialogInstance dialog) =>
        dialog.Close(DialogResult.Ok(true));

    /// <summary>
    /// Close dialog with failure
    /// </summary>
    /// <param name="dialog">Dialog to close</param>
    public static void CloseFailure(this IMudDialogInstance dialog) =>
        dialog.Close(DialogResult.Ok(false));
}