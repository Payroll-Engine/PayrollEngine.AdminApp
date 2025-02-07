using System;
using System.Threading.Tasks;
using MudBlazor;

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Extension methods for <see cref="IDialogService"/>
/// </summary>
public static class DialogServiceExtensions
{
    /// <summary>
    /// Show exception message box
    /// </summary>
    /// <param name="service">Dialog service</param>
    /// <param name="title">Dialog title</param>
    /// <param name="exception">Dialog error</param>
    public static async Task ShowMessageBox(this IDialogService service, string title, Exception exception) =>
        await service.ShowMessageBox(title, exception.GetBaseException().Message);
}