using Microsoft.Extensions.Configuration;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Extension methods for <see cref="IConfigurationRoot"/>
/// </summary>
public static class ConfigurationExtensions
{
    /// <param name="configuration">Configuration</param>
    extension(IConfigurationRoot configuration)
    {
        /// <summary>
        /// Get app culture
        /// </summary>
        public string Culture() =>
            configuration[nameof(Culture)];

        /// <summary>
        /// Get help url
        /// </summary>
        public string AppUrl() =>
            configuration[nameof(AppUrl)];
    }
}