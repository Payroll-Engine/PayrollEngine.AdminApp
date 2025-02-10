using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Navigation;
using Microsoft.Extensions.Configuration;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Interaction logic for AppAboutWindow.xaml
/// </summary>
public partial class AppAboutWindow
{
    private const string AppTitleText = "Payroll Engine Admin";
    private const string AppNameText = "Payroll Engine Admin";
    private const string CopyrightText = "{0} Software Consulting Giannoudis";
    private const string VersionText = "Version {0}";

    /// <summary>
    /// Default constructor
    /// </summary>
    public AppAboutWindow()
    {
        InitializeComponent();
        InitializeWindow();
    }

    /// <summary>
    /// Initialize the app about window
    /// </summary>
    private void InitializeWindow()
    {
        // title
        Title = AppTitleText;
        AppName.Text = AppNameText;

        // copyright
        Copyright.Text = GetCopyright();

        // version
        AppVersion.Text = GetVersion();

        // url
        var url = GetAppUrl();
        UrlLink.NavigateUri = new(url);
        UrlLink.TextDecorations = null;
        UrlLinkLabel.Text = url;
        UrlLink.RequestNavigate += HyperlinkClick;

        // keyboard
        KeyUp += (_, args) =>
        {
            if (args.Key == Key.Escape)
            {
                Close();
            }
        };

        // button
        CloseButton.Click += (_, _) => Close();
    }

    private void HyperlinkClick(object sender, RequestNavigateEventArgs e) =>
        OperatingSystem.StartProcess(GetAppUrl());

    private static string GetAppUrl()
    {
        var appUrl = ResourceTool.GetService<IConfigurationRoot>()?.HelpUrl();
        if (string.IsNullOrWhiteSpace(appUrl))
        {
            appUrl = Specification.DefaultHelpUrl;
        }

        return appUrl;
    }

    /// <summary>
    /// Get application copyright
    /// </summary>
    /// <returns></returns>
    private static string GetCopyright()
    {
        var copyRight = string.Format(CopyrightText, DateTime.Now.Year);
        var assembly = Assembly.GetEntryAssembly();
        if (assembly?.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true).FirstOrDefault()
                is AssemblyCopyrightAttribute copyRightAttribute)
        {
            copyRight = copyRightAttribute.Copyright;
        }
        return copyRight;
    }

    /// <summary>
    /// Get application version
    /// </summary>
    private static string GetVersion()
    {
        var version = FileVersionInfo.GetVersionInfo(typeof(AppAboutWindow).Assembly.Location).FileVersion;
        return string.Format(VersionText, version);
    }
}