using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Asset service
/// </summary>
public interface IAssetService
{
    #region Asset

    /// <summary>
    /// Asset context
    /// </summary>
    AssetContext AssetContext { get; }

    /// <summary>
    /// On-premise backend asset
    /// </summary>
    BackendAsset Backend { get; }

    /// <summary>
    /// Remote backend asset
    /// </summary>
    RemoteBackendAsset RemoteBackend { get; }

    /// <summary>
    /// Wab app asset
    /// </summary>
    WebAppAsset WebApp { get; }

    /// <summary>
    /// Console asset
    /// </summary>
    ConsoleAsset Console { get; }

    /// <summary>
    /// Tools asset
    /// </summary>
    TestsAsset Tests { get; }

    /// <summary>
    /// Examples asset
    /// </summary>
    ExamplesAsset Examples { get; }

    /// <summary>
    /// Load all assets
    /// </summary>
    Task LoadAssetsAsync();

    #endregion

    #region Status

    /// <summary>
    /// Service status
    /// </summary>
    bool ValidStatus { get; }

    /// <summary>
    /// Invalidate the service status
    /// </summary>
    Task InvalidateStatusAsync();

    /// <summary>
    /// Status invalidated event
    /// </summary>
    event EventHandler StatusInvalidated;

    /// <summary>
    /// Update the service status
    /// </summary>
    Task UpdateStatusAsync();

    #endregion

    #region Database

    /// <summary>
    /// Get the database create scripts
    /// </summary>
    Task<List<string>> GetCreateScriptsAsync();

    /// <summary>
    /// Get the database update scripts
    /// </summary>
    Task<List<string>> GetUpdateScriptsAsync(Version existing);

    #endregion

}