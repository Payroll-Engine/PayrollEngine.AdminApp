using System;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Webserver;

/// <summary>
/// Http webserver service
/// </summary>
public class WebserverService : IWebserverService
{
    /// <summary>
    /// Http webserver service
    /// </summary>
    /// <param name="configService">Configuration service</param>
    public WebserverService(IWebserverConfigurationService configService)
    {
        // timeout
        var configTimeout = configService.GetConnectionTimeout();
        if (configTimeout <= 0)
        {
            configTimeout = Specification.HttpConnectTimeoutDefault;
        }
        Timeout = TimeSpan.FromSeconds(configTimeout);
    }

    private TimeSpan Timeout { get; }

    /// <inheritdoc />
    public async Task<WebserverStatus> GetStatusAsync(WebserverConnection connection, IErrorService errorService = null)
    {
        var url = connection.ToUrl();
        if (string.IsNullOrWhiteSpace(url))
        {
            return WebserverStatus.UndefinedConnection;
        }
        if (!await AvailableUrlAsync(url, errorService))
        {
            return WebserverStatus.NotAvailable;
        }

        return WebserverStatus.Available;
    }

    /// <summary>
    /// Test if web url can be connected
    /// </summary>
    /// <param name="url">The webserver url</param>
    /// <param name="errorService">Error service</param>
    /// <returns>true if the connection is opened</returns>
    /// <remarks>source: https://stackoverflow.com/a/16171261</remarks>
    private async Task<bool> AvailableUrlAsync(string url, IErrorService errorService = null)
    {
        try
        {
            using HttpClient client = new();
            client.Timeout = Timeout;
            var result = await client.GetAsync(url);
            switch (result.StatusCode)
            {
                case HttpStatusCode.Accepted:
                case HttpStatusCode.OK:
                    return true;
                default:
                    return false;
            }
        }
        catch (Exception exception)
        {
            errorService?.AddError(exception);
            Debug.WriteLine(exception);
            return false;
        }
    }
}