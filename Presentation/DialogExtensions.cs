using MudBlazor;

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Extension methods for dialogs
/// </summary>
public static class DialogExtensions
{
    /// <param name="dialog">Dialog to close</param>
    extension(IMudDialogInstance dialog)
    {
        /// <summary>
        /// Close dialog with success
        /// </summary>
        public void CloseSuccess() =>
            dialog.Close(DialogResult.Ok(true));

        /// <summary>
        /// Close dialog with failure
        /// </summary>
        public void CloseFailure() =>
            dialog.Close(DialogResult.Ok(false));
    }
}