namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Backend asset parameters
/// </summary>
public class BackendAssetParameters : IAssetParameter
{
    // used for json serialization
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Webserver name
    /// </summary>
    public string WebserverName { get; set; }

    /// <summary>
    /// Webserver executable
    /// </summary>
    public string WebserverExec { get; set; }

    /// <summary>
    /// Database parameter
    /// </summary>
    public DatabaseAssetParameter Database { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <inheritdoc />
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(WebserverExec))
        {
            throw new AdminException("Missing webserver parameter.");
        }

        if (Database == null)
        {
            throw new AdminException("Missing database parameter.");

        }
        Database.Validate();
    }
}