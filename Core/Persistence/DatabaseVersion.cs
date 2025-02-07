namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Database version
/// </summary>
public class DatabaseVersion
{
    /// <summary>
    /// Version table name
    /// </summary>
    public const string TableName = "Version";

    /// <summary>
    /// Major version number
    /// </summary>
    public int MajorVersion { get; set; }

    /// <summary>
    /// Minor version number
    /// </summary>
    public int MinorVersion { get; set; }

    /// <summary>
    /// Sub version number
    /// </summary>
    public int SubVersion { get; set; }
}