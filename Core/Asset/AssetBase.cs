using System;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Asset base class
/// </summary>
public abstract class AssetBase
{
    /// <summary>
    /// Asset name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Asset available state
    /// </summary>
    public bool Available { get; set; }

    /// <summary>
    /// Update the asset status
    /// </summary>
    /// <param name="context">Asset context</param>
    // ReSharper disable once UnusedParameter.Global
    public abstract Task UpdateStatusAsync(AssetContext context);

    /// <summary>
    /// Load asset
    /// </summary>
    /// <param name="context">Asset context</param>
    /// <param name="parameters">Asset parameters</param>
    public virtual Task LoadAsync(AssetContext context,
        Dictionary<string, object> parameters = null) => Task.CompletedTask;

    /// <summary>
    /// Load asset parameters
    /// </summary>
    /// <typeparam name="T">Parameters type</typeparam>
    /// <param name="parameters">Asset parameters</param>
    protected T LoadParameters<T>(Dictionary<string, object> parameters) where T : class, IAssetParameter, new()
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        var json = JsonSerializer.Serialize(parameters);
        T result = null;
        if (!string.IsNullOrWhiteSpace(json))
        {
            result = JsonSerializer.Deserialize<T>(json);
        }
        if (result == null)
        {
            throw new AdminException("Invalid console asset parameters.");
        }
        result.Validate();
        return result;
    }

    public override string ToString() => Name;
}