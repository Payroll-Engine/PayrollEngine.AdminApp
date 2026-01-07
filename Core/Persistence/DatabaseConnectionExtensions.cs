using System;
using System.Text;

namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Extension methods for <see cref="DatabaseConnection"/>
/// </summary>
public static class DatabaseConnectionExtensions
{

    #region Connection

    /// <summary>
    /// Get the database host
    /// </summary>
    /// <param name="connection">Database connection</param>
    public static DatabaseHost? GetHost(this DatabaseConnection connection)
    {
        if (string.IsNullOrWhiteSpace(connection.Server))
        {
            return null;
        }

        // server name localhost or machine name
        if (string.Equals("localhost", connection.Server, StringComparison.InvariantCultureIgnoreCase) ||
            Environment.MachineName.Equals(connection.Server, StringComparison.InvariantCultureIgnoreCase))
        {
            return DatabaseHost.Local;
        }
        return DatabaseHost.Remote;
    }

    #endregion

    #region Convert

    /// <summary>
    /// Convert to connection string.
    /// </summary>
    /// <param name="connection">Connection to convert</param>
    /// <param name="encryptPassword">Encrypt the user password (default: false)</param>
    public static string ToConnectionString(this DatabaseConnection connection, bool encryptPassword = false)
    {
        var buffer = new StringBuilder();

        // server and database
        AppendConnectionItem(buffer, nameof(DatabaseConnection.Server), connection.Server);
        AppendConnectionItem(buffer, nameof(DatabaseConnection.Database), connection.Database);

        // security
        if (connection.IntegratedSecurity)
        {
            AppendConnectionItem(buffer, "Integrated Security", "true");
        }
        if (connection.TrustedConnection)
        {
            AppendConnectionItem(buffer, "TrustServerCertificate", "true");
        }
        else
        {
            AppendConnectionItem(buffer, "User ID", connection.UserId);
            if (!string.IsNullOrWhiteSpace(connection.Password))
            {
                AppendConnectionItem(buffer, nameof(DatabaseConnection.Password),
                    encryptPassword ? "***" : connection.Password);
            }
        }

        // connection timeout
        if (connection.Timeout != 15)
        {
            AppendConnectionItem(buffer, nameof(DatabaseConnection.Timeout), connection.Timeout.ToString());
        }

        // custom parameters
        foreach (var parameter in connection.CustomParameters)
        {
            AppendConnectionItem(buffer, parameter.Name, parameter.Value);
        }

        return buffer.ToString();
    }

    private static void AppendConnectionItem(StringBuilder buffer, string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
        {
            return;
        }
        buffer.Append($"{key}={value}; ");
    }

    /// <summary>
    /// Convert connection string to database connection
    /// </summary>
    /// <param name="connectionString">Connection string</param>
    public static DatabaseConnection ToDatabaseConnection(this string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return null;
        }

        var connection = new DatabaseConnection();
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

            // server
            if (string.Equals(name, nameof(DatabaseConnection.Server), StringComparison.InvariantCultureIgnoreCase))
            {
                connection.Server = value;
                continue;
            }

            // database
            if (string.Equals(name, nameof(DatabaseConnection.Database), StringComparison.InvariantCultureIgnoreCase))
            {
                connection.Database = value;
                continue;
            }

            // timeout
            if (string.Equals(name, nameof(DatabaseConnection.Timeout), StringComparison.InvariantCultureIgnoreCase))
            {
                if (int.TryParse(value, out var timeout) && timeout > 0)
                {
                    connection.Timeout = timeout;
                }
                continue;
            }

            // security
            if (string.Equals(name, "Integrated Security", StringComparison.InvariantCultureIgnoreCase))
            {
                connection.IntegratedSecurity = bool.TrueString.Equals(value, StringComparison.InvariantCultureIgnoreCase);
                continue;
            }
            if (string.Equals(name, "TrustServerCertificate", StringComparison.InvariantCultureIgnoreCase))
            {
                connection.TrustedConnection = bool.TrueString.Equals(value, StringComparison.InvariantCultureIgnoreCase) ||
                                               "SSPI".Equals(value, StringComparison.InvariantCultureIgnoreCase);

                continue;
            }

            // user
            if (string.Equals(name, "User ID", StringComparison.InvariantCultureIgnoreCase))
            {
                connection.UserId = value;
                continue;
            }

            // password
            if (string.Equals(name, nameof(DatabaseConnection.Password), StringComparison.InvariantCultureIgnoreCase))
            {
                connection.Password = value;
                continue;
            }

            // custom parameters
            connection.CustomParameters.SetValue(name, value);
        }

        return connection;
    }

    #endregion

    #region Initialize

    /// <param name="connection">Database connection</param>
    extension(DatabaseConnection connection)
    {
        /// <summary>
        /// Initialize new database
        /// </summary>
        /// <param name="host">Target host</param>
        public void Initialize(DatabaseHost host)
        {
            switch (host)
            {
                case DatabaseHost.Local:
                    connection.InitializeToLocal();
                    break;
                case DatabaseHost.Remote:
                    connection.InitializeToRemote();
                    break;
            }
        }

        private void InitializeToLocal()
        {
            connection.Server = "localhost";
            connection.Database = nameof(PayrollEngine);
            connection.Timeout = 30;
            connection.UserId = null;
            connection.Password = null;
            connection.IntegratedSecurity = true;
            connection.TrustedConnection = true;
            connection.CustomParameters.Clear();
        }

        private void InitializeToRemote()
        {
            connection.Server = null;
            connection.Database = null;
            connection.Timeout = 100;
            connection.UserId = null;
            connection.Password = null;
            connection.IntegratedSecurity = false;
            connection.TrustedConnection = false;
            connection.CustomParameters.Clear();
        }
    }

    #endregion

}