using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using PayrollEngine.AdminApp.Asset;
using PayrollEngine.AdminApp.Persistence;
using PayrollEngine.AdminApp.Persistence.SqlServer;
using PayrollEngine.AdminApp.Presentation;
using PayrollEngine.AdminApp.Setting;
using PayrollEngine.AdminApp.Webserver;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    /// <summary>
    /// Startup handler
    /// </summary>
    /// <param name="sender">Event source</param>
    /// <param name="args">Event argument</param>
    private void Application_Startup(object sender, StartupEventArgs args)
    {
        try
        {
            // application services
            var services = RegisterServices();

            // service provider
            var serviceProvider = services.BuildServiceProvider();
            Resources.Add("ServiceProvider", serviceProvider);

            // load assets
            var assetService = serviceProvider.GetService<IAssetService>();
            if (assetService == null)
            {
                MessageBox.Show("Missing asset service registration");
                return;
            }
            assetService.LoadAssetsAsync().Wait();

        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.GetBaseException().Message);
        }
    }

    /// <summary>
    /// Register application services
    /// </summary>
    private static IServiceCollection RegisterServices()
    {
        var services = new ServiceCollection();

        // blazor web view
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

        // services configuration
        services.AddSingleton<ServiceConfigurationService>();
        services.AddSingleton<IDatabaseConfigurationService, ServiceConfigurationService>();
        services.AddSingleton<IWebserverConfigurationService, ServiceConfigurationService>();
        services.AddSingleton<IFileAssetConfigurationService, ServiceConfigurationService>();

        // localization
        services.AddLocalization(o => { o.ResourcesPath = "Resources"; });

        // base services
        var errorService = new ErrorService();
        services.AddSingleton<IErrorService>(errorService);
        services.AddSingleton<IStatusMessageService, StatusMessageService>();
        services.AddSingleton<IStatusUpdateService, StatusUpdateService>();

        // asset services
        services.AddSingleton<ISettingsService, EnvironmentSettingsService>();
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<IWebserverService, WebserverService>();
        services.AddSingleton<IAssetService, FileAssetService>();

        // presentation services
        services.AddPresentation();

        return services;
    }
}