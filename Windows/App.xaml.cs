using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

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
            var services = new ServiceCollection();

            // application services
            services.AddAppServices();

            // service provider
            var serviceProvider = services.BuildServiceProvider();
            Resources.Add("ServiceProvider", serviceProvider);
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.GetBaseException().Message);
        }
    }
}