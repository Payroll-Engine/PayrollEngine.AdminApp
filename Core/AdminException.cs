using System;

namespace PayrollEngine.AdminApp;

/// <summary>
/// Admin app base exception
/// </summary>
public class AdminException : Exception
{
    public AdminException()
    {
    }

    public AdminException(string message) :
        base(message)
    {
    }

    public AdminException(string message, Exception innerException) :
        base(message, innerException)
    {
    }
}