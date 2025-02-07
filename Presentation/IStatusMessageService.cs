using System;

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Status message service
/// </summary>
public interface IStatusMessageService
{
    /// <summary>
    /// Message type
    /// </summary>
    StatusMessageType MessageType { get; }

    /// <summary>
    /// Message
    /// </summary>
    string Message { get; }

    /// <summary>
    /// Message changed event
    /// </summary>
    event EventHandler MessageChanged;

    /// <summary>
    /// Set status message
    /// </summary>
    /// <param name="message">Message text</param>
    /// <param name="type">Message type</param>
    void SetMessage(string message, StatusMessageType type = StatusMessageType.Information);

    /// <summary>
    /// Set error message
    /// </summary>
    /// <param name="exception">Error</param>
    void SetError(Exception exception);

    /// <summary>
    /// Clear status message
    /// </summary>
    void ClearMessage();
}