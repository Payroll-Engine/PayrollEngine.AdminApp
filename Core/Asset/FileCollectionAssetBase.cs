using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Base class for file collection assets
/// </summary>
/// <param name="fileProvider">File provider</param>
public abstract class FileCollectionAssetBase(PhysicalFileProvider fileProvider) :
    AssetBase, IFolderAsset
{
    private PhysicalFileProvider FileProvider { get; } = fileProvider;

    /// <inheritdoc />
    public Task BrowseAsync()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            return Task.CompletedTask;
        }

        var assetFolder = OperatingSystem.PathCombine(FileProvider.Root, Name);
        var assetFileProvider = new PhysicalFileProvider(assetFolder);
        var assetFiles = assetFileProvider.GetDirectoryContents(string.Empty);
        if (assetFiles.Any())
        {
            // open file explorer
            OperatingSystem.StartProcess(Name);
        }
        return Task.CompletedTask;
    }
}