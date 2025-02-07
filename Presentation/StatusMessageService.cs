using System;

namespace PayrollEngine.AdminApp.Presentation;

/// <inheritdoc />
public class StatusMessageService : IStatusMessageService
{
    /// <inheritdoc />
    public StatusMessageType MessageType { get; private set; }

    /// <inheritdoc />
    public string Message { get; private set; }

    /// <inheritdoc />
    public event EventHandler MessageChanged;

    /// <inheritdoc />
    public void SetMessage(string message, StatusMessageType type = StatusMessageType.Information)
    {
        if (string.Equals(Message, message))
        {
            return;
        }

        Message = message;
        MessageType = type;
        MessageChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    public void SetError(Exception exception) =>
        SetMessage(exception.GetBaseException().Message, StatusMessageType.Error);

    /// <inheritdoc />
    public void ClearMessage()
    {
        Message = null;
        MessageType = StatusMessageType.Information;
    }
}