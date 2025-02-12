using System;

namespace PayrollEngine.AdminApp.Presentation;

/// <inheritdoc />
public class StatusUpdateService : IStatusUpdateService
{
    /// <summary>
    /// Update counter
    /// </summary>
    private int UpdateCount { get; set; }

    /// <inheritdoc />
    public bool Updating => UpdateCount > 0;

    /// <inheritdoc />
    public void BeginUpdate()
    {
        UpdateCount++;
    }

    /// <inheritdoc />
    public void EndUpdate()
    {
        if (!Updating)
        {
            throw new InvalidOperationException("Unbalanced status update");
        }
        UpdateCount--;
    }
}