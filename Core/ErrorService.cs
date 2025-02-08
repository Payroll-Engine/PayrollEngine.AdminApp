using System;
using System.Text;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp;

/// <inheritdoc />
public class ErrorService : IErrorService
{
    private List<string> Errors { get; } = [];

    /// <inheritdoc />
    public string RetrieveErrors()
    {
        var buffer = new StringBuilder();
        for (var i = Errors.Count - 1; i >= 0; i--)
        {
            buffer.AppendLine(Errors[i]);
        }
        Reset();
        return buffer.ToString();
    }

    /// <inheritdoc />
    public void AddError(Exception exception) =>
        Errors.Add(exception?.GetBaseException().Message);

    /// <inheritdoc />
    public void Reset() => Errors.Clear();
}