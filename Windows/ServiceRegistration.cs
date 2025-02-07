using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.WebServer;
using PayrollEngine.AdminApp.Presentation;
using PayrollEngine.AdminApp.Persistence.SqlServer;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// App service registration
/// </summary>
internal static class ServiceRegistration
{
    /// <summary>
    /// Add application services
    /// </summary>
    /// <param name="services"></param>
    public static void AddAppServices(this IServiceCollection services)
    {
        // web view
        services.AddWpfBlazorWebView();
#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif

        // Mud Blazor
        services.AddMudServices();

        // app configuration
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<App>();
        var configuration = builder.Build();
        services.AddSingleton(configuration);

        // localization
        services.AddLocalization(o => { o.ResourcesPath = "Resources"; });
        services.AddTransient<Localizer>();

        // base services
        var errorService = new ErrorService();
        services.AddSingleton<IErrorService>(errorService);
        services.AddSingleton<IStatusMessageService, StatusMessageService>();

        // assets
        AddAssetService(services, configuration, errorService);

        // presentation services
        services.AddPresentation();
    }

    /// <summary>
    /// Add asset service
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">App configuration</param>
    /// <param name="errorService">Error service</param>
    private static void AddAssetService(IServiceCollection services, IConfigurationRoot configuration,
        IErrorService errorService)
    {
        // base services
        var settingsService = new EnvironmentSettingsService();
        var databaseService = new DatabaseService(errorService);
        var webServerService = new WebServerService(errorService,
            configuration.HttpConnectTimeout(WebServerService.DefaultTimeout));

        // file system asset path
        var assetPath = configuration.FileAssetsPath();
        if (string.IsNullOrWhiteSpace(assetPath) || !OperatingSystem.DirectoryExists(assetPath))
        {
            throw new AdminException($"Invalid or missing file asset path {assetPath}");
        }

        // asset service
        var fullPath = OperatingSystem.DirectoryFullName(assetPath);
        var service = new FileAssetService(
            fileProvider: new(fullPath),
            settingsService: settingsService,
            databaseService: databaseService,
            webServerService: webServerService);
        service.LoadAssetsAsync().Wait();
        services.AddSingleton<IAssetService>(service);
    }
}