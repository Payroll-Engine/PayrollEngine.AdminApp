using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.Webserver;

/// <summary>
/// Webserver service
/// </summary>
public interface IWebserverService
{
    /// <summary>
    /// Get the webserver status
    /// </summary>
    /// <param name="connection">Webserver connection</param>
    /// <param name="errorService">Error service</param>
    Task<WebserverStatus> GetStatusAsync(WebserverConnection connection, IErrorService errorService = null);
}