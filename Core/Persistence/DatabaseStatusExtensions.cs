namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Extension methods for <see cref="DatabaseStatus"/>
/// </summary>
public static class DatabaseStatusExtensions
{
    /// <param name="status">Database status</param>
    extension(DatabaseStatus status)
    {
        /// <summary>
        /// Test if database needs to be created or updated
        /// </summary>
        public bool PendingChange() => 
            status.ReadyToCreate() || status.ReadyToUpdate();

        /// <summary>
        /// Test if database is ready to create
        /// </summary>
        public bool ReadyToCreate() =>
            status is DatabaseStatus.MissingDatabase or DatabaseStatus.EmptyDatabase;

        /// <summary>
        /// Test if database is ready to update
        /// </summary>
        public bool ReadyToUpdate() =>
            status is DatabaseStatus.OutdatedVersion;
    }
}
