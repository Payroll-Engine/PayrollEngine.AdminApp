namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Backend asset parameters
/// </summary>
public class BackendAssetParameters : IAssetParameter
{
    // used for json serialization
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Web server executable name
    /// </summary>
    public string WebServerExec { get; set; }

    /// <summary>
    /// Database parameter
    /// </summary>
    public DatabaseAssetParameter Database { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <inheritdoc />
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(WebServerExec))
        {
            throw new AdminException("Missing web server parameter.");
        }

        if (Database == null)
        {
            throw new AdminException("Missing database parameter.");

        }
        Database.Validate();
    }
}