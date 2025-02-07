using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp.Persistence;

/// <summary>
/// Collection of database connection parameter
/// </summary>
public class ConnectionParameterCollection : IEnumerable<ConnectionParameter>
{
    private readonly List<ConnectionParameter> parameters = [];

    /// <summary>
    /// The number of parameters
    /// </summary>
    private int Count => parameters.Count;

    /// <summary>
    /// Get parameter value by name
    /// </summary>
    /// <param name="name">Parameter name</param>
    public string this[string name] => GetValue(name);

    /// <summary>
    /// Get parameter
    /// </summary>
    /// <param name="name">Parameter name</param>
    private ConnectionParameter Get(string name) =>
        parameters.FirstOrDefault(x => string.Equals(x.Name, name));

    /// <summary>
    /// Set parameter
    /// </summary>
    /// <param name="parameter">Parameter to set</param>
    public void Set(ConnectionParameter parameter)
    {
        if (parameters.Contains(parameter))
        {
            return;
        }
        parameters.Add(parameter);
    }

    /// <summary>
    /// Check for existing parameter
    /// </summary>
    /// <param name="name">Parameter to test</param>
    public bool Contains(string name) =>
        parameters.Any(x => string.Equals(x.Name, name));

    /// <summary>
    /// Get parameter value
    /// </summary>
    /// <param name="name">Parameter name</param>
    private string GetValue(string name) =>
        Get(name)?.Value;

    /// <summary>
    /// Set parameter value
    /// </summary>
    /// <param name="name">Parameter name</param>
    /// <param name="value">Parameter value</param>
    public void SetValue(string name, string value) =>
        Set(new(name, value));

    public void Remove(string name)
    {
        var existing = Get(name);
        if (existing != null)
        {
            parameters.Remove(existing);
        }
    }

    /// <summary>
    /// Remove parameter
    /// </summary>
    /// <param name="parameter">Parameter to remove</param>
    public void Remove(ConnectionParameter parameter)
    {
        if (parameters.Contains(parameter))
        {
            parameters.Remove(parameter);
        }
    }

    /// <summary>
    /// Clear all parameters
    /// </summary>
    public void Clear() => parameters.Clear();

    /// <summary>
    /// Test for equal values
    /// </summary>
    /// <param name="compare">Object to compare</param>
    public bool EqualValues(ConnectionParameterCollection compare)
    {
        if (parameters.Count != compare.Count)
        {
            return false;
        }

        foreach (var parameter in compare)
        {
            var value = GetValue(parameter.Name);
            if (!string.Equals(value, parameter.Value))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Import parameter values
    /// </summary>
    /// <param name="source">Object to import</param>
    public void ImportValues(ConnectionParameterCollection source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        parameters.Clear();
        foreach (var parameter in source)
        {
            Set(parameter);
        }
    }

    #region IEnumerator

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<ConnectionParameter> GetEnumerator() =>
        parameters.GetEnumerator();

    #endregion

    public override string ToString() =>
        $"{Count} parameters";
}