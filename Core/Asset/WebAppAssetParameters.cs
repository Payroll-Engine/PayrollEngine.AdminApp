namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Web app asset parameter
/// </summary>
public class WebAppAssetParameters : IAssetParameter
{
    // used for json serialization
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Web server executable name
    /// </summary>
    public string WebServerExec { get; set; }

    /// <inheritdoc />
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(WebServerExec))
        {
            throw new AdminException("Missing web server parameter.");
        }
    }
}
