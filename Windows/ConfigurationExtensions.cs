using System;
using Microsoft.Extensions.Configuration;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Extension methods for <see cref="IConfigurationRoot"/>
/// </summary>
public static class ConfigurationExtensions
{
    /// <summary>
    /// Get file asset path
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public static string FileAssetsPath(this IConfigurationRoot configuration) =>
        configuration[nameof(FileAssetsPath)];

    /// <summary>
    /// Get help url
    /// </summary>
    /// <param name="configuration">Configuration</param>
    public static string HelpUrl(this IConfigurationRoot configuration) =>
        configuration[nameof(HelpUrl)];

    /// <summary>
    /// Get http connection timeout
    /// </summary>
    /// <param name="configuration">Configuration</param>
    /// <param name="defaultTimeout">Default http request timeout</param>
    public static TimeSpan HttpConnectTimeout(this IConfigurationRoot configuration, TimeSpan defaultTimeout)
    {
        var timeout = configuration[nameof(HttpConnectTimeout)];
        if (string.IsNullOrWhiteSpace(timeout) || !int.TryParse(timeout, out var seconds))
        {
            return defaultTimeout;
        }
        return TimeSpan.FromSeconds(seconds);
    }
}