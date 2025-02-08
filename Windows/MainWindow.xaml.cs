using System;
using System.Windows;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    private const string defaultHelpUrl = "https://github.com/Payroll-Engine";

    /// <summary>
    /// Default constructor
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        InitializeCulture();
        InitializeWindow();
    }

    /// <summary>
    /// Start app help
    /// </summary>
    private void Help()
    {
        var appUrl = ResourceTool.GetService<IConfigurationRoot>()?.HelpUrl();
        if (string.IsNullOrWhiteSpace(appUrl))
        {
            appUrl = defaultHelpUrl;
        }
        OperatingSystem.StartProcess(appUrl);
    }

    /// <summary>
    /// Get app version
    /// </summary>
    private string GetAppVersion()
    {
        // version
        var assemblyInfo = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location);
        if (string.IsNullOrWhiteSpace(assemblyInfo.FileVersion))
        {
            return null;
        }
        return $"v{assemblyInfo.FileVersion}";
    }

    /// <summary>
    /// Initialize the application culture
    /// </summary>
    private void InitializeCulture()
    {
        var culture = ResourceTool.GetService<IConfigurationRoot>()?.Culture();
        if (string.IsNullOrWhiteSpace(culture))
        {
            return;
        }

        try
        {
            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }
        catch (Exception exception)
        {
            MessageBox.Show(Title, exception.GetBaseException().Message);
        }
    }

    /// <summary>
    /// Initialize the application window
    /// </summary>
    private void InitializeWindow()
    {
        // title
        Title = "Payroll Engine Admin";

        if (OperatingSystem.IsAdministrator())
        {
            AdminText.Text = "ADMIN";
            AdminText.Visibility = Visibility.Visible;
        }
        else
        {
            AdminText.Visibility = Visibility.Hidden;
        }

        // buttons
        AppVersion.Text = GetAppVersion();
        HelpButton.Click += (_, _) => Help();
        CloseButton.Click += (_, _) => Close();
    }
}