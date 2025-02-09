namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Extension methods for <see cref="DatabaseStatus"/>
/// </summary>
public static class DatabaseStatusExtensions
{
    /// <summary>
    /// Test if database needs to be created or updated
    /// </summary>
    /// <param name="status">Database status</param>
    public static bool PendingChange(this DatabaseStatus status) =>
        ReadyToCreate(status) || ReadyToUpdate(status);

    /// <summary>
    /// Test if database is ready to create
    /// </summary>
    /// <param name="status">Database status</param>
    public static bool ReadyToCreate(this DatabaseStatus status) =>
        status is DatabaseStatus.MissingDatabase or DatabaseStatus.EmptyDatabase;

    /// <summary>
    /// Test if database is ready to update
    /// </summary>
    /// <param name="status">Database status</param>
    public static bool ReadyToUpdate(this DatabaseStatus status) =>
        status is DatabaseStatus.OutdatedVersion;
}
