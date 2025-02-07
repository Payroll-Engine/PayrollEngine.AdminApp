using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;

namespace PayrollEngine.AdminApp.Presentation.Components.Layout;

/// <summary>
/// Application layout base class
/// </summary>
public abstract class MainLayoutBase : LayoutComponentBase
{
    [Inject] private IConfigurationRoot Configuration { get; set; }

    /// <summary>
    /// Theme provider
    /// </summary>
    protected MudThemeProvider ThemeProvider { get; set; }

    // the default dark mode value controls the startup flickering
    /// <summary>
    /// Dark mode
    /// </summary>
    protected bool DarkMode { get; private set; } = true;

    /// <summary>
    /// Initialize dark mode
    /// </summary>
    private async Task InitDarkModeAsync()
    {
        var darkMode = GetDarkModeConfig() ?? await ThemeProvider.GetSystemPreference();

        // no dark mode change
        if (DarkMode == darkMode)
        {
            return;
        }

        DarkMode = darkMode;
        StateHasChanged();
    }

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitDarkModeAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    /// <summary>
    /// Get dark mode configuration
    /// </summary>
    private bool? GetDarkModeConfig()
    {
        var value = Configuration[Specification.DarkModeConfig];
        if (!string.IsNullOrWhiteSpace(value) &&
            bool.TryParse(value, out var darkModeResult))
        {
            return darkModeResult;
        }

        return null;
    }
}