using System;

namespace PayrollEngine.AdminApp;

/// <summary>
/// Application error service
/// </summary>
public interface IErrorService
{
    /// <summary>
    /// Retrieve errors 
    /// </summary>
    /// <remarks>Errors are removed</remarks>
    /// <returns>Collected error string or null</returns>
    string RetrieveErrors();

    /// <summary>
    /// Add application exception
    /// </summary>
    /// <param name="exception">Exception</param>
    void AddError(Exception exception);

    /// <summary>
    /// Remove all errors, ignoring error watches
    /// </summary>
    void Reset();
}