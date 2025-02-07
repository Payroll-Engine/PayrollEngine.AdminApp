using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.WebServer;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Asse service suing the local file system
/// </summary>
public class FileAssetService : IAssetService
{
    public FileAssetService(
        PhysicalFileProvider fileProvider,
        ISettingsService settingsService,
        IDatabaseService databaseService,
        IWebServerService webServerService)
    {
        FileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));

        // assets
        Backend = new();
        RemoteBackend = new();
        WebApp = new();
        Console = new();
        Tests = new(fileProvider);
        Examples = new(fileProvider);
        Assets = [Backend, RemoteBackend, WebApp, Console, Tests, Examples];

        // status
        AssetContext = new AssetContext
        {
            SettingsService = settingsService,
            DatabaseService = databaseService,
            WebServerService = webServerService
        };
    }

    #region Asset

    private List<AssetBase> Assets { get; }
    private PhysicalFileProvider FileProvider { get; }

    /// <inheritdoc />
    public AssetContext AssetContext { get; }

    /// <inheritdoc />
    public BackendAsset Backend { get; }
    
    /// <inheritdoc />
    public RemoteBackendAsset RemoteBackend { get; }
    
    /// <inheritdoc />
    public WebAppAsset WebApp { get; }
    
    /// <inheritdoc />
    public ConsoleAsset Console { get; }
    
    /// <inheritdoc />
    public TestsAsset Tests { get; }
    
    /// <inheritdoc />
    public ExamplesAsset Examples { get; }

    #endregion

    #region Status

    /// <inheritdoc />
    public bool ValidStatus { get; private set; } = true;
    
    /// <inheritdoc />
    public event EventHandler StatusInvalidated;

    /// <inheritdoc />
    public Task InvalidateStatusAsync()
    {
        ValidStatus = false;
        StatusInvalidated?.Invoke(this, EventArgs.Empty);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task UpdateStatusAsync()
    {
        foreach (var asset in Assets)
        {
            if (asset.Available)
            {
                await asset.UpdateStatusAsync(AssetContext);
            }
        }
        ValidStatus = true;
    }

    #endregion

    #region Database

    /// <inheritdoc />
    public Task<List<string>> GetCreateScriptsAsync()
    {
        var scripts = new List<string>();
        foreach (var script in Backend.Parameters.Database.InitScripts)
        {
            scripts.Add(ReadScriptFile(script));
        }
        return Task.FromResult(scripts);
    }

    /// <inheritdoc />
    public Task<List<string>> GetUpdateScriptsAsync(Version existing)
    {
        var scripts = new List<string>();

        var scriptsByVersion = Backend.Parameters.Database.UpdateScripts.OrderBy(x => x.FromVersion);
        foreach (var updateScript in scriptsByVersion)
        {
            if (updateScript.FromVersion < existing)
            {
                continue;
            }
            foreach (var script in updateScript.Scripts)
            {
                scripts.Add(ReadScriptFile(script));
            }
        }
        return Task.FromResult(scripts);
    }

    private string ReadScriptFile(string name)
    {
        var fileName = OperatingSystem.PathCombine(Backend.Name, name);
        if (!OperatingSystem.FileExists(fileName))
        {
            throw new AdminException($"Missing database script file {fileName}");
        }
        return File.ReadAllText(fileName);
    }

    #endregion

    #region Lifecycle

    /// <summary>
    /// Load all assets
    /// </summary>
    public async Task LoadAssetsAsync()
    {
        foreach (var asset in Assets)
        {
            var parameters = LoadFileAsset(asset);
            if (asset.Available)
            {
                await asset.LoadAsync(AssetContext, parameters);
            }
        }

        // ensure only one enabled backend asset
        RemoteBackend.Available = !Backend.Available;
    }

    private Dictionary<string, object> LoadFileAsset(AssetBase asset)
    {
        // don't load virtual assets
        if (asset is IVirtualAsset)
        {
            return null;
        }

        var name = asset.GetType().Name;
        if (name.EndsWith(nameof(Asset)))
        {
            name = name.Substring(0, name.Length - nameof(Asset).Length);
        }

        // check for files within the asset folder
        var assetFolder = OperatingSystem.PathCombine(FileProvider.Root, name);

        IDirectoryContents files = null;
        if (OperatingSystem.DirectoryExists(assetFolder))
        {
            var assetFileProvider = new PhysicalFileProvider(assetFolder);
            files = assetFileProvider.GetDirectoryContents(string.Empty);
        }

        // reset
        if (files == null || !files.Any())
        {
            asset.Name = null;
            asset.Available = false;
            return null;
        }

        // asset parameters
        Dictionary<string, object> parameters = null;
        var assetParameterFile = OperatingSystem.PathCombine(assetFolder, Specification.AssetParameterFileName);
        if (OperatingSystem.FileExists(assetParameterFile))
        {
            parameters = OperatingSystem.DeserializeJsonFile<Dictionary<string, object>>(assetParameterFile);
        }

        asset.Name = assetFolder;
        asset.Available = true;
        return parameters;
    }

    #endregion

}