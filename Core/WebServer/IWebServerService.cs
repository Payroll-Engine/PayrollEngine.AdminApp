using System.Threading.Tasks;

namespace PayrollEngine.AdminApp.WebServer;

/// <summary>
/// Web server service
/// </summary>
public interface IWebServerService
{
    /// <summary>
    /// Get the web server status
    /// </summary>
    /// <param name="connection">Web server connection</param>
    Task<WebServerStatus> GetStatusAsync(WebServerConnection connection);
}