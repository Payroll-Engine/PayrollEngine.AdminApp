using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Presentation localizations
/// </summary>
/// <param name="factory"></param>
public class Localizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    // app
    public string AppTitle => PropertyValue();
    public string AppLoadingMessage => PropertyValue();
    public string AppWelcomeMessage => PropertyValue();
    public string RefreshAppStatusHelp => PropertyValue();
    public string AutoRefreshAppStatusHelp(int seconds) =>
        FormatValue(PropertyValue(), nameof(seconds), seconds);
    public string LastUpdatedAppStatusHelp => PropertyValue();
    public string MissingAssetsError => PropertyValue();

    // common
    public string Close => PropertyValue();
    public string Apply => PropertyValue();
    public string Cancel => PropertyValue();
    public string Ok => PropertyValue();
    public string Start => PropertyValue();
    public string StartHelp => PropertyValue();
    public string Edit => PropertyValue();
    public string Add => PropertyValue();
    public string Create => PropertyValue();
    public string Update => PropertyValue();
    public string Continue => PropertyValue();
    public string UrlUndefined => PropertyValue();
    public string CopyToClipboardHelp => PropertyValue();
    public string Name => PropertyValue();
    public string Value => PropertyValue();
    public string NoEditChangesMessage => PropertyValue();
    public MarkupString MissingDotNetDevCertificate => new(PropertyValue());
    public MarkupString DotNetDevCertificateSetupFailed => new(PropertyValue());

    // database
    public string Database => PropertyValue();
    public string DatabaseConnectionDialogTitle => PropertyValue();
    public string ConnectionParameterDialogTitle => PropertyValue();
    public string DatabaseCreateDialogTitle => PropertyValue();
    public string DatabaseUpdateDialogTitle => PropertyValue();
    public string RefreshDatabaseStatusHelp => PropertyValue();
    public string CommonFieldsTab => PropertyValue();
    public string ServerName => PropertyValue();
    public string DatabaseName => PropertyValue();
    public string Timeout => PropertyValue();
    public string SecurityTab => PropertyValue();
    public string IntegratedSecurity => PropertyValue();
    public string TrustedConnection => PropertyValue();
    public string UserId => PropertyValue();
    public string Password => PropertyValue();
    public string CustomParametersTab => PropertyValue();
    public string DatabaseCollation => PropertyValue();
    public string NoParametersAvailable => PropertyValue();
    public string InvalidDatabaseConnection => PropertyValue();
    public string DatabaseUpdateHelp => PropertyValue();
    public string DatabaseStatusHelp => PropertyValue();
    public MarkupString DatabaseSetupHelp => new(PropertyValue());
    public string InvalidConnectionParameter(string name) =>
        FormatValue(PropertyValue(), nameof(name), name);
    public string RemoveParameterQuery(string name) =>
        FormatValue(PropertyValue(), nameof(name), name);
    public string DatabaseConnectionUpdateMessage => PropertyValue();
    public string DatabaseCreateSuccessMessage => PropertyValue();
    public string DatabaseCreateErrorMessage => PropertyValue();
    public string DatabaseUpdateSuccessMessage => PropertyValue();
    public string DatabaseUpdateErrorMessage => PropertyValue();
    public string DatabaseInstallInfo => PropertyValue();
    public string ScriptInstallInfo => PropertyValue();
    public string DatabaseHostTypeQuery => PropertyValue();
    public string DatabaseNotEmptyMessage => PropertyValue();

    public string DatabaseCreateError(string value) =>
        FormatValue(PropertyValue(), nameof(value), value);
    public string DatabaseSetupError(string value) =>
        FormatValue(PropertyValue(), nameof(value), value);

    // webserver
    public string Webserver => PropertyValue();
    public string WebserverDialogTitle => PropertyValue();
    public string BaseUrl => PropertyValue();
    public string Port => PropertyValue();
    public string ApiKey => PropertyValue();
    public string WebserverConnectionUpdateMessage => PropertyValue();

    // web app
    public string WebAppTitle => PropertyValue();
    public string OpenWebApp => PropertyValue();
    public string OpenWebAppHelp => PropertyValue();

    // backend
    public string BackendLocalTitle => PropertyValue();
    public string BackendRemoteTitle => PropertyValue();
    public string OpenBackend => PropertyValue();
    public string OpenBackendHelp => PropertyValue();

    // console
    public string ConsoleTitle => PropertyValue();
    public string BrowseFolder => PropertyValue();
    public string BrowseFolderHelp => PropertyValue();
    // console: file type
    public string Register => PropertyValue();
    public string RegisterHelp => PropertyValue();
    public string AdminRegisterHelp => PropertyValue();
    public string CurrentRegistration(string name) =>
        FormatValue(PropertyValue(), nameof(name), name);
    public string FileTypeRegisterMessage(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string FileTypeRegisterQuery(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string FileTypeRegisterTitle => PropertyValue();
    public string FileTypeRegisterSuccess => PropertyValue();
    public string FileTypeRegisterError => PropertyValue();
    public string Unregister => PropertyValue();
    public string FileTypeUnregisterQuery(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string FileTypeUnregisterTitle => PropertyValue();
    public string FileTypeUnregisterSuccess => PropertyValue();
    public string FileTypeUnregisterError => PropertyValue();

    // other assets
    public string TestsTitle => PropertyValue();
    public string ExamplesTitle => PropertyValue();
}