using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.AdminApp.Asset;

namespace PayrollEngine.AdminApp.Presentation.Components.Assets;

/// <summary>
/// View for the file asset
/// </summary>
public abstract class FileAssetViewBase : ComponentBase
{
    /// <summary>
    /// File asset
    /// </summary>
    [Parameter] public AssetBase Asset { get; set; }

    /// <summary>
    /// Asset title
    /// </summary>
    [Parameter] public string Title { get; set; }

    [Inject] protected Localizer Localizer { get; set; }

    /// <summary>
    /// Browse the files
    /// </summary>
    protected async Task BrowseAsync()
    {
        if (Asset is not IBrowseAsset browseAsset)
        {
            throw new InvalidOperationException();
        }
        await browseAsset.BrowseAsync();
    }
}