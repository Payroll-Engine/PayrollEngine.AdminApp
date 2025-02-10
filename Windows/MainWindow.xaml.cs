using System;
using System.Windows;
using System.Threading;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace PayrollEngine.AdminApp.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
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

        // window buttons
        InfoButton.Click += (_, _) => ShowAbout();
        CloseButton.Click += (_, _) => Close();
    }

    private void ShowAbout() =>
        new AppAboutWindow { Owner = this }.ShowDialog();
}