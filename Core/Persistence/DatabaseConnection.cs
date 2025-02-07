using System;
using System.Linq;

namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Database connection
/// </summary>
public class DatabaseConnection
{
    /// <summary>
    /// Server name
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    /// Database name
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// User id
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// User password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Timeout in seconds
    /// </summary>
    public int Timeout { get; set; } = 15;

    /// <summary>
    /// USe integrated security
    /// </summary>
    public bool IntegratedSecurity { get; set; }

    /// <summary>
    /// Trusted connection
    /// </summary>
    public bool TrustedConnection { get; set; }

    /// <summary>
    /// Custom connection parameters
    /// </summary>
    public ConnectionParameterCollection CustomParameters { get; } = new();

    public DatabaseConnection()
    {
    }

    public DatabaseConnection(DatabaseConnection copy)
    {
        ImportValues(copy);
    }

    /// <summary>
    /// Test for empty connection
    /// </summary>
    public bool IsEmpty() =>
        Server == null &&
        Database == null &&
        UserId == null &&
        Password == null &&
        Timeout == 15 &&
        IntegratedSecurity == false &&
        TrustedConnection == false &&
        !CustomParameters.Any();

    /// <summary>
    /// Test for valid connection
    /// </summary>
    public bool HasRequiredValues()
    {
        if (string.IsNullOrWhiteSpace(Server) || string.IsNullOrWhiteSpace(Database))
        {
            return false;
        }

        if (TrustedConnection)
        {
            return true;
        }
        return !string.IsNullOrWhiteSpace(UserId) && !string.IsNullOrWhiteSpace(Password);
    }

    /// <summary>
    /// Test for equal values
    /// </summary>
    /// <param name="compare">Object to compare</param>
    public bool EqualValues(DatabaseConnection compare) =>
        compare != null &&
        Equals(Server, compare.Server) &&
        Equals(Database, compare.Database) &&
        Equals(UserId, compare.UserId) &&
        Equals(Password, compare.Password) &&
        Equals(Timeout, compare.Timeout) &&
        Equals(IntegratedSecurity, compare.IntegratedSecurity) &&
        Equals(TrustedConnection, compare.TrustedConnection) &&
        CustomParameters.EqualValues(compare.CustomParameters);

    /// <summary>
    /// Import connection values
    /// </summary>
    /// <param name="source">Object to import</param>
    public void ImportValues(DatabaseConnection source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        Server = source.Server;
        Database = source.Database;
        UserId = source.UserId;
        Password = source.Password;
        Timeout = source.Timeout;
        IntegratedSecurity = source.IntegratedSecurity;
        TrustedConnection = source.TrustedConnection;
        CustomParameters.ImportValues(source.CustomParameters);
    }

    public void SetDefaultValues()
    {
        Server = Specification.DatabaseServerName;
        Database = Specification.DatabaseName;
        Timeout = Specification.DatabaseConnectionTimeout;
        UserId = null;
        Password = null;
        IntegratedSecurity = Specification.DatabaseDefaultIntegratedSecurity;
        TrustedConnection = Specification.DatabaseTrustedConnection;
        CustomParameters.Clear();
    }

    public override string ToString() =>
        string.IsNullOrWhiteSpace(Server) && string.IsNullOrWhiteSpace(Database) ?
            string.Empty : $"{Server}:{Database}";
}