using System;

namespace PayrollEngine.AdminApp;

/// <summary>
/// Application error service
/// </summary>
public interface IErrorService
{
    /// <summary>
    /// Check for available errors
    /// </summary>
    bool HasErrors { get; }

    /// <summary>
    /// Get error history
    /// </summary>
    /// <param name="clearHistory">Clear error history</param>
    string GetErrorHistory(bool clearHistory = false);

    /// <summary>
    /// Add application exception
    /// </summary>
    /// <param name="exception">Exception</param>
    void AddError(Exception exception);

    /// <summary>
    /// Remove all errors, ignoring error watches
    /// </summary>
    void ClearErrorHistory();

    /// <summary>
    /// Add error watch
    /// </summary>
    /// <param name="watchName">Watch name</param>
    void AddWatch(string watchName);

    /// <summary>
    /// Get watch error history
    /// </summary>
    /// <param name="watchName"></param>
    /// <param name="queryMode"></param>
    string GetErrorHistory(string watchName, ErrorWatchQueryMode queryMode = ErrorWatchQueryMode.KeepHistory);
}