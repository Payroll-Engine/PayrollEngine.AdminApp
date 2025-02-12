using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PayrollEngine.AdminApp.Webserver;
using PayrollEngine.AdminApp.Persistence;

namespace PayrollEngine.AdminApp.Setting;

/// <summary>
/// Asset configuration service storing the values within the system environment
/// </summary>
public class EnvironmentSettingsService : ISettingsService
{

    /// <inheritdoc />
    public Task<Dictionary<string, string>> GetEnvironmentSettingsAsync()
    {
        var environment = new Dictionary<string, string>();
        AddEnvironmentVariable(environment, Specification.PayrollDatabaseConnection);
        AddEnvironmentVariable(environment, Specification.PayrollApiKey);
        AddEnvironmentVariable(environment, Specification.PayrollApiConnection);
        AddEnvironmentVariable(environment, Specification.PayrollWebAppConnection);
        return Task.FromResult(environment);
    }

    private static void AddEnvironmentVariable(Dictionary<string, string> environment, string variable)
    {
        var value = GetUserVariable(variable);
        if (!string.IsNullOrWhiteSpace(value))
        {
            environment.Add(variable, value);
        }
    }

    #region Database

    /// <inheritdoc />
    public Task<DatabaseConnection> GetDatabaseConnectionAsync()
    {
        var variable = GetUserVariable(Specification.PayrollDatabaseConnection);
        if (string.IsNullOrWhiteSpace(variable))
        {
            return Task.FromResult(new DatabaseConnection());
        }
        var connection = variable.ToDatabaseConnection();
        return Task.FromResult(connection);
    }

    /// <inheritdoc />
    public Task SetDatabaseConnectionAsync(DatabaseConnection connection)
    {
        if (connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        var variable = connection.ToConnectionString();
        if (string.IsNullOrWhiteSpace(variable))
        {
            return Task.CompletedTask;
        }

        SetUserVariable(Specification.PayrollDatabaseConnection, variable);
        return Task.CompletedTask;
    }

    #endregion

    #region Api Key

    /// <inheritdoc />
    public Task<string> GetApiKeyAsync() =>
        Task.FromResult(GetUserVariable(Specification.PayrollApiKey));

    /// <inheritdoc />
    public Task SetApiKeyAsync(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException(nameof(apiKey));
        }

        SetUserVariable(Specification.PayrollApiKey, apiKey);
        return Task.CompletedTask;
    }

    #endregion

    #region Api Connection

    /// <inheritdoc />
    public Task<WebserverConnection> GetApiConnectionAsync()
    {
        var variable = GetUserVariable(Specification.PayrollApiConnection);
        if (string.IsNullOrWhiteSpace(variable))
        {
            return Task.FromResult<WebserverConnection>(null);
        }
        var connection = variable.ToWebserverConnection();
        return Task.FromResult(connection);
    }

    /// <inheritdoc />
    public Task SetApiConnectionAsync(WebserverConnection connection)
    {
        if (connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        var variable = connection.ToConnectionString();
        if (string.IsNullOrWhiteSpace(variable))
        {
            return Task.FromResult<WebserverConnection>(null);
        }

        SetUserVariable(Specification.PayrollApiConnection, variable);
        return Task.CompletedTask;
    }

    #endregion

    #region Web App Connection

    /// <inheritdoc />
    public Task<WebserverConnection> GetWebAppConnectionAsync()
    {
        var variable = GetUserVariable(Specification.PayrollWebAppConnection);
        var connection = variable.ToWebserverConnection();
        return Task.FromResult(connection);
    }

    /// <inheritdoc />
    public Task SetWebAppConnectionAsync(WebserverConnection connection)
    {
        if (connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        var variable = connection.ToConnectionString();
        if (string.IsNullOrWhiteSpace(variable))
        {
            return Task.CompletedTask;
        }

        SetUserVariable(Specification.PayrollWebAppConnection, variable);
        return Task.CompletedTask;
    }

    #endregion

    private static string GetUserVariable(string variableName) =>
        Environment.GetEnvironmentVariable(variableName, EnvironmentVariableTarget.User);

    private static void SetUserVariable(string variableName, string variableValue) =>
        Environment.SetEnvironmentVariable(variableName, variableValue, EnvironmentVariableTarget.User);

}