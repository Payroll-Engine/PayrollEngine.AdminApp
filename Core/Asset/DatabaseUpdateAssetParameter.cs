using System;
using System.Linq;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Asset database update parameter
/// </summary>
public class DatabaseUpdateAssetParameter : IAssetParameter
{
    // used for json serialization
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// From database version
    /// </summary>
    public Version FromVersion { get; set; }

    /// <summary>
    /// To database version
    /// </summary>
    public Version ToVersion { get; set; }

    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    // ReSharper disable once CollectionNeverUpdated.Global
    /// <summary>
    /// Update scripts
    /// </summary>
    public List<string> Scripts { get; set; } = [];
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    /// <inheritdoc />
    public void Validate()
    {
        if (FromVersion.IsEmpty())
        {
            throw new AdminException("Missing database update from version.");
        }
        if (ToVersion.IsEmpty())
        {
            throw new AdminException("Missing database update to version.");
        }

        if (FromVersion.Equals(ToVersion))
        {
            throw new AdminException("Database parameter from/to version is invalid.");
        }

        if (!Scripts.Any())
        {
            throw new AdminException("Missing database update scripts.");
        }
    }
}