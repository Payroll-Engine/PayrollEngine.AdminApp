namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Console asset parameters
/// </summary>
public class ConsoleAssetParameters : IAssetParameter
{
    // used for json serialization
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Executable name
    /// </summary>
    public string Executable { get; set; }
    
    /// <summary>
    /// File type name
    /// </summary>
    public string FileTypeName { get; set; }
    
    /// <summary>
    /// File type extension
    /// </summary>
    public string FileTypeExtension { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <inheritdoc />
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Executable))
        {
            throw new AdminException("Missing executable parameter.");
        }
        if (string.IsNullOrWhiteSpace(FileTypeName))
        {
            throw new AdminException("Missing file type name parameter.");
        }
        if (string.IsNullOrWhiteSpace(FileTypeExtension))
        {
            throw new AdminException("Missing file type extension parameter.");
        }
    }
}
