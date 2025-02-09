using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace PayrollEngine.AdminApp.Presentation.Components;

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
    public static async Task ShowErrorMessage(this IDialogService service, string title, Exception exception) =>
        await service.ShowMessage(title, exception.GetBaseException().Message,
            buttonColor: Color.Error);

    /// <summary>
    /// Show message box
    /// </summary>
    /// <param name="service">Dialog service</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    /// <param name="buttonColor">Button color</param>
    /// <param name="buttonText">Button text</param>
    public static async Task ShowMessage(this IDialogService service, string title, string message,
        Color buttonColor = Color.Default, string buttonText = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(nameof(title));
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(nameof(message));
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(MessageDialog.Message), message },
            { nameof(MessageDialog.ButtonColor), buttonColor },
            { nameof(MessageDialog.ButtonText), buttonText }
        };

        // show message dialog
        await service.ShowAsync<MessageDialog>(title, parameters);
    }

    /// <summary>
    /// Show message dialog
    /// </summary>
    /// <param name="service">Dialog service</param>
    /// <param name="title">Dialog title</param>
    /// <param name="markupMessage">Dialog message</param>
    /// <param name="buttonColor">Button color</param>
    /// <param name="buttonText">Button text</param>
    public static async Task ShowMessage(this IDialogService service, string title,
        MarkupString markupMessage, Color buttonColor = Color.Default, string buttonText = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(nameof(title));
        }
        if (string.IsNullOrWhiteSpace(markupMessage.Value))
        {
            throw new ArgumentException(nameof(markupMessage));
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(MessageDialog.MarkupMessage), markupMessage },
            { nameof(MessageDialog.ButtonColor), buttonColor },
            { nameof(MessageDialog.ButtonText), buttonText }
        };

        // show message dialog
        await service.ShowAsync<MessageDialog>(title, parameters);
    }

    /// <summary>
    /// Show confirmation dialog
    /// </summary>
    /// <param name="service">Dialog service</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    /// <param name="okButtonColor">Ok button color</param>
    /// <param name="okButtonText">Ok button text</param>
    /// <param name="cancelButtonColor">Cancel button color</param>
    /// <param name="cancelButtonText">Cancel button text</param>
    public static async Task<bool?> ShowConfirm(this IDialogService service, string title, string message,
        Color okButtonColor = Color.Primary, string okButtonText = null,
        Color cancelButtonColor = Color.Default, string cancelButtonText = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(nameof(title));
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(nameof(message));
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(ConfirmDialog.Message), message },
            { nameof(ConfirmDialog.OkButtonColor), okButtonColor },
            { nameof(ConfirmDialog.OkButtonText), okButtonText },
            { nameof(ConfirmDialog.CancelButtonColor), cancelButtonColor },
            { nameof(ConfirmDialog.CancelButtonText), cancelButtonText }
        };

        // show message dialog
        var result = await (await service.ShowAsync<ConfirmDialog>(title, parameters)).Result;
        if (result?.Data == null)
        {
            return null;
        }
        return (bool?)result.Data;
    }

    /// <summary>
    /// Show confirmation dialog
    /// </summary>
    /// <param name="service">Dialog service</param>
    /// <param name="title">Dialog title</param>
    /// <param name="markupMessage">Dialog message</param>
    /// <param name="okButtonColor">Ok button color</param>
    /// <param name="okButtonText">Ok button text</param>
    /// <param name="cancelButtonColor">Cancel button color</param>
    /// <param name="cancelButtonText">Cancel button text</param>
    public static async Task<bool?> ShowConfirm(this IDialogService service,
        string title, MarkupString markupMessage,
        Color okButtonColor = Color.Primary, string okButtonText = null,
        Color cancelButtonColor = Color.Default, string cancelButtonText = null)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(nameof(title));
        }
        if (string.IsNullOrWhiteSpace(markupMessage.Value))
        {
            throw new ArgumentException(nameof(markupMessage));
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(ConfirmDialog.MarkupMessage), markupMessage },
            { nameof(ConfirmDialog.OkButtonColor), okButtonColor },
            { nameof(ConfirmDialog.OkButtonText), okButtonText },
            { nameof(ConfirmDialog.CancelButtonColor), cancelButtonColor },
            { nameof(ConfirmDialog.CancelButtonText), cancelButtonText }
        };

        // show message dialog
        var result = await (await service.ShowAsync<ConfirmDialog>(title, parameters)).Result;
        if (result?.Data == null)
        {
            return null;
        }
        return (bool?)result.Data;
    }

    /// <summary>
    /// Show enum selection dialog
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="service">Dialog service</param>
    /// <param name="title">Dialog title</param>
    /// <param name="message">Dialog message</param>
    /// <param name="hideCancel">Hide cancel button</param>
    /// <param name="reverseOrder">Reverse button order (default: enum value)</param>
    /// <param name="buttonColor">Button color</param>
    /// <param name="cancelButtonColor">Cancel button color</param>
    /// <param name="localizerMap">Localizer map</param>
    public static async Task<T?> ShowEnumSelect<T>(this IDialogService service, string title, string message,
        bool hideCancel = false, bool reverseOrder = false,
        Color buttonColor = Color.Primary, Color cancelButtonColor = Color.Default,
        string localizerMap = null) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException(nameof(title));
        }
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException(nameof(message));
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(EnumSelectDialog<T>.Message), message },
            { nameof(EnumSelectDialog<T>.HideCancel), hideCancel },
            { nameof(EnumSelectDialog<T>.ReverseOrder), reverseOrder },
            { nameof(EnumSelectDialog<T>.ButtonColor), buttonColor },
            { nameof(EnumSelectDialog<T>.CancelButtonColor), cancelButtonColor },
            { nameof(EnumSelectDialog<T>.LocalizerMap), localizerMap }
        };

        // show enum dialog
        var result = await (await service.ShowAsync<EnumSelectDialog<T>>(title, parameters)).Result;
        if (result?.Data == null)
        {
            return null;
        }
        return (T)result.Data;
    }
}