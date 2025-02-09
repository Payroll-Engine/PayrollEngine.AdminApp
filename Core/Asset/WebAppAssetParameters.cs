namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Web app asset parameter
/// </summary>
public class WebAppAssetParameters : IAssetParameter
{
    // used for json serialization
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Webserver name
    /// </summary>
    public string WebserverName { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Webserver executable name
    /// </summary>
    public string WebserverExec { get; set; }

    /// <inheritdoc />
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(WebserverExec))
        {
            throw new AdminException("Missing webserver parameter.");
        }
    }
}
