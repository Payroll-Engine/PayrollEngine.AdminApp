using System;
using System.Text;

namespace PayrollEngine.AdminApp.WebServer;

/// <summary>
/// Extension methods for <see cref="WebServerConnection"/>
/// </summary>
public static class WebServerConnectionExtensions
{
    /// <summary>
    /// Test for local and secure connection
    /// </summary>
    /// <param name="connection">Connection to test</param>
    public static bool IsLocalSecureConnection(this WebServerConnection connection)
    {
        if (string.IsNullOrWhiteSpace(connection.BaseUrl))
        {
            return false;
        }

        var url = new Uri(connection.BaseUrl);
        return string.Equals(url.Authority, "localhost", StringComparison.InvariantCultureIgnoreCase) &&
               string.Equals(url.Scheme, "https", StringComparison.InvariantCultureIgnoreCase);
    }

    /// <summary>
    /// Convert to connection string.
    /// </summary>
    /// <param name="connection">Connection to convert</param>
    /// <param name="encryptApiKey">Encrypt the api key (default: false)</param>
    public static string ToConnectionString(this WebServerConnection connection, bool encryptApiKey = false)
    {
        if (!connection.HasRequiredValues())
        {
            return null;
        }

        var buffer = new StringBuilder();

        // base url
        buffer.Append($"{nameof(WebServerConnection.BaseUrl)}={connection.BaseUrl}; ");

        // port
        if (connection.Port != 0)
        {
            buffer.Append($"{nameof(WebServerConnection.Port)}={connection.Port}; ");
        }

        // timeout
        if (connection.Timeout != TimeSpan.Zero)
        {
            buffer.Append($"{nameof(WebServerConnection.Timeout)}={connection.Timeout}; ");
        }

        // api key
        if (!string.IsNullOrWhiteSpace(connection.ApiKey))
        {
            var apiKey = encryptApiKey ? "***" : connection.ApiKey;
            buffer.Append($"{nameof(WebServerConnection.ApiKey)}={apiKey}; ");
        }
        return buffer.ToString();
    }

    /// <summary>
    /// Convert connection string to web server connection
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    public static WebServerConnection ToWebServerConnection(this string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return null;
        }

        var connection = new WebServerConnection();
        var tokens = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var token in tokens)
        {
            var valueTokens = token.Split('=', StringSplitOptions.RemoveEmptyEntries);
            if (valueTokens.Length != 2)
            {
                continue;
            }

            var name = valueTokens[0].Trim();
            var value = valueTokens[1].Trim();
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            {
                continue;
            }

            // base url
            if (string.Equals(name, nameof(WebServerConnection.BaseUrl), StringComparison.InvariantCultureIgnoreCase))
            {
                connection.BaseUrl = value;
                continue;
            }

            // port
            if (string.Equals(name, nameof(WebServerConnection.Port), StringComparison.InvariantCultureIgnoreCase))
            {
                if (int.TryParse(value, out var port))
                {
                    connection.Port = port;
                }
                continue;
            }

            // timeout
            if (string.Equals(name, nameof(WebServerConnection.Timeout), StringComparison.InvariantCultureIgnoreCase))
            {
                if (TimeSpan.TryParse(value, out var timeout))
                {
                    connection.Timeout = timeout;
                }
                continue;
            }

            // api key
            if (string.Equals(name, nameof(WebServerConnection.ApiKey), StringComparison.InvariantCultureIgnoreCase))
            {
                connection.ApiKey = value;
            }
        }
        return connection;
    }
}