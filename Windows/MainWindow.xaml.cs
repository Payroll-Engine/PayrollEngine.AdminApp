using System.Windows;
using System.Diagnostics;
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
}