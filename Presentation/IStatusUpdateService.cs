namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Status update service
/// </summary>
public interface IStatusUpdateService
{
    /// <summary>
    /// Update indicator
    /// </summary>
    bool Updating { get; }

    /// <summary>
    /// Begin update
    /// </summary>
    void BeginUpdate();

    /// <summary>
    /// End update
    /// </summary>
    void EndUpdate();
}