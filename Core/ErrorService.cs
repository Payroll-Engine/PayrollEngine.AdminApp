using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp;

public class ErrorService : IErrorService
{
    private Dictionary<string, List<string>> Watches { get; } = [];
    private List<string> Errors { get; } = [];

    /// <inheritdoc />
    public bool HasErrors => Errors.Any();

    /// <inheritdoc />
    public string GetErrorHistory(bool clearHistory = false)
    {
        var errors = CollectErrors(Errors);
        if (clearHistory)
        {
            ClearErrorHistory();
        }
        return errors;
    }

    /// <inheritdoc />
    public void AddError(Exception exception) =>
        AddError(exception?.GetBaseException().Message);

    /// <inheritdoc />
    public void ClearErrorHistory() => Errors.Clear();

    private void AddError(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
        {
            return;
        }
        Errors.Add(error);
        foreach (var watch in Watches)
        {
            watch.Value.Add(error);
        }
    }

    /// <inheritdoc />
    public void AddWatch(string watchName)
    {
        if (string.IsNullOrWhiteSpace(watchName))
        {
            throw new ArgumentException(nameof(watchName));
        }
        if (Watches.ContainsKey(watchName))
        {
            return;
        }
        Watches.Add(watchName, []);
    }

    /// <inheritdoc />
    public string GetErrorHistory(string watchName,
        ErrorWatchQueryMode queryMode = ErrorWatchQueryMode.KeepHistory)
    {
        if (string.IsNullOrWhiteSpace(watchName))
        {
            throw new ArgumentException(nameof(watchName));
        }
        if (!Watches.TryGetValue(watchName, out var watch))
        {
            return null;
        }
        var errors = CollectErrors(watch);
        switch (queryMode)
        {
            case ErrorWatchQueryMode.ClearHistory:
                ClearWatchHistory(watchName);
                break;
            case ErrorWatchQueryMode.RemoveWatch:
                RemoveWatch(watchName);
                break;
            case ErrorWatchQueryMode.KeepHistory:
            default:
                break;
        }
        return errors;
    }

    private void ClearWatchHistory(string watchName)
    {
        if (string.IsNullOrWhiteSpace(watchName))
        {
            throw new ArgumentException(nameof(watchName));
        }
        if (Watches.ContainsKey(watchName))
        {
            Watches.Clear();
        }
    }

    private void RemoveWatch(string watchName)
    {
        if (string.IsNullOrWhiteSpace(watchName))
        {
            throw new ArgumentException(nameof(watchName));
        }
        if (Watches.ContainsKey(watchName))
        {
            Watches.Remove(watchName);
        }
    }

    private string CollectErrors(List<string> errors)
    {
        if (errors.Count == 0)
        {
            return null;
        }

        var buffer = new StringBuilder();
        for (var i = errors.Count - 1; i >= 0; i--)
        {
            buffer.AppendLine(errors[i]);
        }

        return buffer.ToString();
    }
}