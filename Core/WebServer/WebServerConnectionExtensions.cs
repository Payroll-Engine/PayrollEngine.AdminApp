using System;
using System.Text;

namespace PayrollEngine.AdminApp.Webserver;

/// <summary>
/// Extension methods for <see cref="WebserverConnection"/>
/// </summary>
public static class WebserverConnectionExtensions
{
    /// <param name="connection">Connection to test</param>
    extension(WebserverConnection connection)
    {
        /// <summary>
        /// Test for local and secure connection
        /// </summary>
        public bool IsLocalSecureConnection()
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
        /// <param name="encryptApiKey">Encrypt the api key (default: false)</param>
        public string ToConnectionString(bool encryptApiKey = false)
        {
            if (!connection.HasRequiredValues())
            {
                return null;
            }

            var buffer = new StringBuilder();

            // base url
            buffer.Append($"{nameof(WebserverConnection.BaseUrl)}={connection.BaseUrl}; ");

            // port
            if (connection.Port != 0)
            {
                buffer.Append($"{nameof(WebserverConnection.Port)}={connection.Port}; ");
            }

            // timeout
            if (connection.Timeout != TimeSpan.Zero)
            {
                buffer.Append($"{nameof(WebserverConnection.Timeout)}={connection.Timeout}; ");
            }

            // api key
            if (!string.IsNullOrWhiteSpace(connection.ApiKey))
            {
                var apiKey = encryptApiKey ? "***" : connection.ApiKey;
                buffer.Append($"{nameof(WebserverConnection.ApiKey)}={apiKey}; ");
            }
            return buffer.ToString();
        }
    }

    /// <summary>
    /// Convert connection string to webserver connection
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    public static WebserverConnection ToWebserverConnection(this string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return null;
        }

        var connection = new WebserverConnection();
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
            if (string.Equals(name, nameof(WebserverConnection.BaseUrl), StringComparison.InvariantCultureIgnoreCase))
            {
                connection.BaseUrl = value;
                continue;
            }

            // port
            if (string.Equals(name, nameof(WebserverConnection.Port), StringComparison.InvariantCultureIgnoreCase))
            {
                if (int.TryParse(value, out var port))
                {
                    connection.Port = port;
                }
                continue;
            }

            // timeout
            if (string.Equals(name, nameof(WebserverConnection.Timeout), StringComparison.InvariantCultureIgnoreCase))
            {
                if (TimeSpan.TryParse(value, out var timeout))
                {
                    connection.Timeout = timeout;
                }
                continue;
            }

            // api key
            if (string.Equals(name, nameof(WebserverConnection.ApiKey), StringComparison.InvariantCultureIgnoreCase))
            {
                connection.ApiKey = value;
            }
        }
        return connection;
    }
}