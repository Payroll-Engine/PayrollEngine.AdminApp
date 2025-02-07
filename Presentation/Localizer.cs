using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Presentation localizations
/// </summary>
/// <param name="factory"></param>
public class Localizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string AppTitle => PropertyValue();
    public string AppLoadingMessage => PropertyValue();
    public string AppWelcomeMessage => PropertyValue();
    public string BackendLocalTitle => PropertyValue();
    public string BackendRemoteTitle => PropertyValue();

    public string WebAppTitle => PropertyValue();
    public string ConsoleTitle => PropertyValue();
    public string TestsTitle => PropertyValue();
    public string ExamplesTitle => PropertyValue();

    public string Close => PropertyValue();
    public string Apply => PropertyValue();
    public string Cancel => PropertyValue();
    public string Ok => PropertyValue();
    public string OpenBackend => PropertyValue();
    public string OpenBackendHelp => PropertyValue();
    public string OpenWebApp => PropertyValue();
    public string OpenWebAppHelp => PropertyValue();
    public string BrowseFolder => PropertyValue();
    public string BrowseFolderHelp => PropertyValue();
    public string Start => PropertyValue();
    public string StartHelp => PropertyValue();
    public string Register => PropertyValue();
    public string RegisterHelp => PropertyValue();
    public string AdminRegisterHelp => PropertyValue();
    public string Unregister => PropertyValue();
    public string CurrentRegistration(string name) =>
        FormatValue(PropertyValue(), nameof(name), name);
    public string EditServer => PropertyValue();
    public string EditServerHelp => PropertyValue();
    public string EditDatabase => PropertyValue();
    public string EditDatabaseHelp => PropertyValue();
    public string CopyToClipboardHelp => PropertyValue();

    public string WebServer => PropertyValue();
    public string Database => PropertyValue();
    public string WebServerDialogTitle => PropertyValue();
    public string DatabaseConnectionDialogTitle => PropertyValue();
    public string ConnectionParameterDialogTitle => PropertyValue();
    public string DatabaseCreateDialogTitle => PropertyValue();
    public string DatabaseUpdateDialogTitle => PropertyValue();
    public string FileTypeRegisterTitle => PropertyValue();
    public string None => PropertyValue();
    public string Setup => PropertyValue();
    public string Add => PropertyValue();
    public string Create => PropertyValue();
    public string Update => PropertyValue();
    public string RefreshAppStatusHelp => PropertyValue();
    public string AutoRefreshAppStatusHelp(int seconds) =>
        FormatValue(PropertyValue(), nameof(seconds), seconds);
    public string LastUpdatedAppStatusHelp => PropertyValue();
    public string RefreshConnectionStatusHelp => PropertyValue();
    public string CommonFields => PropertyValue();
    public string ServerName => PropertyValue();
    public string DatabaseName => PropertyValue();
    public string Timeout => PropertyValue();
    public string IntegratedSecurity => PropertyValue();
    public string TrustedConnection => PropertyValue();
    public string Security => PropertyValue();
    public string UserId => PropertyValue();
    public string Password => PropertyValue();
    public string CustomParameters => PropertyValue();
    public string DatabaseUpdateHelp => PropertyValue();
    public string ConnectionStatusHelp => PropertyValue();
    public string ConnectionUndefined => PropertyValue();
    public string MissingAssetsError => PropertyValue();
    public string Name => PropertyValue();
    public string Value => PropertyValue();
    public string NotDataAvailable => PropertyValue();
    public string BaseUrl => PropertyValue();
    public string Port => PropertyValue();
    public string ApiKey => PropertyValue();

    public MarkupString MissingDotNetDevCertificate => new(PropertyValue());
    public MarkupString DotNetDevCertificateSetupFailed => new(PropertyValue());
    public MarkupString DatabaseSetupHelp => new(PropertyValue());

    public string DuplicateConnectionParameter(string name) =>
        FormatValue(PropertyValue(), nameof(name), name);
    public string ConnectionStatus(string status) =>
        FormatValue(PropertyValue(), nameof(status), status);
    public string RemoveQuery(string name) =>
        FormatValue(PropertyValue(), nameof(name), name);

    public string DatabaseConnectionUpdateMessage => PropertyValue();
    public string WebServerConnectionUpdateMessage => PropertyValue();
    public string NoEditChangesMessage => PropertyValue();
    public string FileTypeRegisterMessage(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string FileTypeRegisterQuery(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string FileTypeRegisterSuccess => PropertyValue();
    public string FileTypeRegisterError => PropertyValue();
    public string FileTypeUnregisterQuery(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string FileTypeUnregisterTitle => PropertyValue();
    public string FileTypeUnregisterSuccess => PropertyValue();
    public string FileTypeUnregisterError => PropertyValue();
    public string DatabaseCreateSuccessMessage => PropertyValue();
    public string DatabaseCreateErrorMessage => PropertyValue();
    public string DatabaseUpdateSuccessMessage => PropertyValue();
    public string DatabaseUpdateErrorMessage => PropertyValue();
    public string ScriptInstallInfo => PropertyValue();
    public string DatabaseInstallInfo => PropertyValue();
    public string DatabaseCollation => PropertyValue();

    public string DatabaseCreateError(string value) =>
        FormatValue(PropertyValue(), nameof(value), value);
    public string DatabaseSetupError(string value) =>
        FormatValue(PropertyValue(), nameof(value), value);
}