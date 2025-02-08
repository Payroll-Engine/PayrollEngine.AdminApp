using System;

namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Database connection parameter
/// </summary>
public class ConnectionParameter
{
    /// <summary>
    /// Parameter name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Parameter value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public ConnectionParameter()
    {
    }

    /// <summary>
    /// Value constructor
    /// </summary>
    /// <param name="name">Parameter name</param>
    /// <param name="value">Parameter value</param>
    public ConnectionParameter(string name, string value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copy">Copy source</param>
    public ConnectionParameter(ConnectionParameter copy)
    {
        ImportValues(copy);
    }

    /// <summary>
    /// Test for valid parameter
    /// </summary>
    public bool HasRequiredValues() =>
        !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Test for equal values
    /// </summary>
    /// <param name="compare">Object to compare</param>
    public bool EqualValues(ConnectionParameter compare) =>
        compare != null &&
        Equals(Name, compare.Name) &&
        Equals(Value, compare.Value);

    /// <summary>
    /// Import parameter values
    /// </summary>
    /// <param name="source">Object to import</param>
    public void ImportValues(ConnectionParameter source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        Name = source.Name;
        Value = source.Value;
    }

    /// <inheritdoc />
    public override string ToString() =>
        $"{Name}={Value}";
}