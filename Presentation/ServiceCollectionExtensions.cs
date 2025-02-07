using Microsoft.Extensions.DependencyInjection;

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register presentation services
    /// </summary>
    /// <param name="services">Service collection</param>
    public static void AddPresentation(this IServiceCollection services)
    {
        services.AddTransient<Localizer>();
    }
}