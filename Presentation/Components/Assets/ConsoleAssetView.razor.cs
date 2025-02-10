using Microsoft.AspNetCore.Components;
using PayrollEngine.AdminApp.Asset;

namespace PayrollEngine.AdminApp.Presentation.Components.Assets;

/// <summary>
/// View for the console asset
/// </summary>
public abstract class ConsoleAssetViewBase : ComponentBase
{
    /// <summary>
    /// Console asset
    /// </summary>
    [Parameter] public ConsoleAsset Asset { get; set; }

    /// <summary>
    /// Localizer
    /// </summary>
    [Inject] protected Localizer Localizer { get; set; }
}