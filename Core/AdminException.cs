using System;

namespace PayrollEngine.AdminApp;

/// <summary>
/// Admin app base exception
/// </summary>
public class AdminException : Exception
{
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="message"></param>
    public AdminException(string message) :
        base(message)
    {
    }
}