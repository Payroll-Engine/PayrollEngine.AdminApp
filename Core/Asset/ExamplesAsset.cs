using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Examples asset
/// </summary>
/// <param name="fileProvider">FIle provider</param>
public class ExamplesAsset(PhysicalFileProvider fileProvider) :
    FileCollectionAssetBase(fileProvider)
{
    /// <inheritdoc />
    public override Task UpdateStatusAsync(AssetContext context) =>
        Task.CompletedTask;
}