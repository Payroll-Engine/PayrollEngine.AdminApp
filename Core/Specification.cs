
namespace PayrollEngine.AdminApp;

/// <summary>
/// Payroll Engine Admin App Specification
/// </summary>
public static class Specification
{
    #region Environment

    /// <summary>Api connection settings environment variable name, containing the payroll configuration</summary>
    public const string PayrollApiConnection = "PayrollApiConnection";

    /// <summary>Api key variable name, containing the backend api key</summary>
    public const string PayrollApiKey = "PayrollApiKey";

    /// <summary>Configuration setting name, containing the payroll database connection string</summary>
    public const string PayrollDatabaseConnection = "PayrollDatabaseConnection";

    /// <summary>Configuration setting name, containing the payroll web app connection string</summary>
    public const string PayrollWebAppConnection = "PayrollWebAppConnection";

    #endregion

    #region Assets

    /// <summary>File name containing the assets parameter</summary>
    public const string AssetParameterFileName = "asset.json";

    /// <summary>Default backend url</summary>
    public const string BackendDefaultBaseUrl = "https://localhost";
    /// <summary>Default backend port</summary>
    public const int BackendDefaultPort = 44354;
    /// <summary>Default backend connection timeout</summary>
    public const int BackendDefaultTimeout = 100;

    /// <summary>Default web app url</summary>
    public const string WebAppDefaultBaseUrl = "https://localhost";
    /// <summary>Default web app url</summary>
    public const int WebAppDefaultPort = 7179;

    #endregion

    #region Database

    /// <summary>Default web app url</summary>
    public const string DatabaseCollation = "SQL_Latin1_General_CP1_CS_AS";

    #endregion

    #region App config

    /// <summary>App dark mode config name</summary>
    public const string DarkModeConfig = "DarkMode";
    /// <summary>Http connection timeout config name</summary>
    public const string HttpConnectTimeoutConfig = "HttpConnectTimeout";

    /// <summary>App refresh default timeout</summary>
    public const int AppRefreshDefaultTimeout = 120;
    /// <summary>App refresh mim timeout</summary>
    public const int AppRefreshMinTimeout = 15;
    /// <summary>App refresh max timeout</summary>
    public const int AppRefreshMaxTimeout = 60 * 60; // 1 hour
    /// <summary>Auto refresh timeout config name</summary>
    public const string AutoRefreshTimeoutConfig = "AutoRefreshTimeout";

    #endregion

}