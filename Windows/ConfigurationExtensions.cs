using Microsoft.Extensions.Configuration;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Extension methods for <see cref="IConfigurationRoot"/>
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Get app culture
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public static string Culture(this IConfigurationRoot configuration) =>
        configuration[nameof(Culture)];

    /// <summary>
    /// Get help url
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public static string AppUrl(this IConfigurationRoot configuration) =>
        configuration[nameof(AppUrl)];
}