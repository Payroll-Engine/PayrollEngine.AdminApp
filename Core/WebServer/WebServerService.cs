using System;
using System.Net;
using System.Net.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.WebServer;
/// <summary>
/// Http web server service
/// </summary>
/// <param name="errorService">Error service</param>
/// <param name="timeout">Test request timeout</param>
public class WebServerService(IErrorService errorService, TimeSpan timeout) : IWebServerService
{
    private IErrorService ErrorService { get; } = errorService;
    public static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);

    private TimeSpan Timeout { get; } =
        timeout == TimeSpan.Zero ? DefaultTimeout : timeout;

    /// <inheritdoc />
    public async Task<WebServerStatus> GetStatusAsync(WebServerConnection connection)
    {
        var url = connection.ToUrl();
        if (string.IsNullOrWhiteSpace(url))
        {
            return WebServerStatus.UndefinedConnection;
        }
        if (!await AvailableUrlAsync(url))
        {
            return WebServerStatus.NotAvailable;
        }

        return WebServerStatus.Available;
    }

    /// <summary>
    /// Test if web url can be connected
    /// </summary>
    /// <param name="url">The web server url</param>
    /// <returns>true if the connection is opened</returns>
    /// <remarks>source: https://stackoverflow.com/a/16171261</remarks>
    private async Task<bool> AvailableUrlAsync(string url)
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
            ErrorService.AddError(exception);
            Debug.WriteLine(exception);
            return false;
        }
    }
}