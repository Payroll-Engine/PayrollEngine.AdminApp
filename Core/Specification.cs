
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

    public const string PayrollWebAppConnection = "PayrollWebAppConnection";

    #endregion

    #region Assets

    public const string AssetParameterFileName = "asset.json";
    
    public const string BackendDefaultBaseUrl = "https://localhost";
    public const int BackendDefaultPort = 44354;
    public const int BackendDefaultTimeout = 100;

    public const string WebAppDefaultBaseUrl = "https://localhost";
    public const int WebAppDefaultPort = 7179;

    #endregion

    #region Database

    public const string TenantTableName = "Tenant";

    public const string DatabaseCollation = "SQL_Latin1_General_CP1_CS_AS";

    public const string DatabaseServerName = "localhost";
    public const string DatabaseName = nameof(PayrollEngine);
    public const bool DatabaseDefaultIntegratedSecurity = true;
    public const bool DatabaseTrustedConnection = true;
    public const int DatabaseConnectionTimeout = 100;

    #endregion

    #region App config

    public const int AppRefreshDefaultTimeout = 120;
    public const int AppRefreshMinTimeout = 15;
    public const int AppRefreshMaxTimeout = 60 * 60; // 1 hour

    public const string DarkModeConfig = "DarkMode";
    public const string HttpConnectTimeoutConfig = "HttpConnectTimeout";
    public const string AutoRefreshTimeoutConfig = "AutoRefreshTimeout";

    #endregion

}