using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Resource tool
/// </summary>
internal static class ResourceTool
{
    private static IServiceProvider GetServiceProvider() =>
        Application.Current.Resources["ServiceProvider"] as ServiceProvider;

    /// <summary>
    /// Get service from service provider
    /// </summary>
    /// <typeparam name="T">Service type</typeparam>
    internal static T GetService<T>() where T : class
    {
        var serviceProvider = GetServiceProvider();
        if (serviceProvider == null)
        {
            return null;
        }

        return serviceProvider.GetService<T>();
    }
}