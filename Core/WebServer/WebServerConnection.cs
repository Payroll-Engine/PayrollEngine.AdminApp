using System;

namespace PayrollEngine.AdminApp.Webserver;

/// <summary>
/// Webserver connection
/// </summary>
public class WebserverConnection
{
    /// <summary>
    /// Base URL (required)
    /// </summary>
    public string BaseUrl { get; set; }

    /// <summary>
    /// Port
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Request timeout
    /// </summary>
    public TimeSpan Timeout { get; set; }

    /// <summary>
    /// Backend API key
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// Default constructor
    /// </summary>
    public WebserverConnection()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copy"></param>
    public WebserverConnection(WebserverConnection copy)
    {
        ImportValues(copy);
    }

    /// <summary>
    /// Convert to url
    /// </summary>
    public string ToUrl() =>
        string.IsNullOrWhiteSpace(BaseUrl) ?
            string.Empty :
            Port != 0 ?
                $"{BaseUrl}:{Port}" :
                BaseUrl;

    /// <summary>
    /// Test for empty connection
    /// </summary>
    public bool IsEmpty() =>
        BaseUrl == null &&
        Port == 0 &&
        Timeout == TimeSpan.Zero &&
        ApiKey == null;

    /// <summary>
    /// Test for valid connection
    /// </summary>
    public bool HasRequiredValues() =>
        !string.IsNullOrEmpty(BaseUrl);

    /// <summary>
    /// Test for equal values
    /// </summary>
    /// <param name="compare">Object to compare</param>
    public bool EqualValues(WebserverConnection compare) =>
        compare != null &&
        Equals(BaseUrl, compare.BaseUrl) &&
        Equals(Port, compare.Port) &&
        Equals(Timeout, compare.Timeout) &&
        Equals(ApiKey, compare.ApiKey);

    /// <summary>
    /// Import connection values
    /// </summary>
    /// <param name="source">Object to import</param>
    public void ImportValues(WebserverConnection source)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        BaseUrl = source.BaseUrl;
        Port = source.Port;
        Timeout = source.Timeout;
        ApiKey = source.ApiKey;
    }

    /// <inheritdoc />
    public override string ToString() => ToUrl();
}