using System;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace PayrollEngine.AdminApp;

/// <summary>
/// Operating system abstraction
/// </summary>
public static class OperatingSystem
{

    #region OS

    /// <summary>
    /// Test for windows OS
    /// </summary>
    public static bool IsWindows() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    /// <summary>
    /// Test for OSX
    /// </summary>
    public static bool IsOsx() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

    /// <summary>
    /// Test for linux OS
    /// </summary>
    private static bool IsLinux() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

    /// <summary>
    /// Test for Free Bsd OS
    /// </summary>
    public static bool IsFreeBsd() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);

    #endregion

    #region File

    /// <summary>
    /// Test for existing file
    /// </summary>
    /// <param name="path">File name to test</param>
    public static bool FileExists(string path) =>
        File.Exists(path);

    /// <summary>
    /// Deserialize JSON file
    /// </summary>
    /// <typeparam name="T">Target object</typeparam>
    /// <param name="fileName">Source file name</param>
    public static T DeserializeJsonFile<T>(string fileName) where T : class
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException(nameof(fileName));
        }
        if (!FileExists(fileName))
        {
            return null;
        }

        var text = File.ReadAllText(fileName);
        var result = JsonSerializer.Deserialize<T>(text);
        return result;
    }

    #endregion

    #region Directory

    /// <summary>
    /// Get the current directory
    /// </summary>
    public static string GetCurrentDirectory() =>
        Directory.GetCurrentDirectory();

    /// <summary>
    /// Get the current parent directory
    /// </summary>
    public static string GetCurrentParentDirectory() =>
        GetParentDirectory(GetCurrentDirectory());

    /// <summary>
    /// Get the application directory
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public static string GetAppDirectory() =>
        GetParentDirectory(System.Reflection.Assembly.GetExecutingAssembly().Location);

    /// <summary>
    /// Get the parent directory
    /// </summary>
    private static string GetParentDirectory(string path) =>
        new DirectoryInfo(path).Parent?.FullName;

    /// <summary>
    /// Test for existing directory
    /// </summary>
    /// <param name="path">File name to test</param>
    public static bool DirectoryExists(string path) =>
        Directory.Exists(path);

    /// <summary>
    /// Get directory full name
    /// </summary>
    /// <param name="path">Directory name</param>
    public static string DirectoryFullName(string path) =>
        new DirectoryInfo(path).FullName;

    /// <summary>
    /// Combine path elements
    /// </summary>
    /// <param name="path1">First path element</param>
    /// <param name="path2">Second path element</param>
    public static string PathCombine(string path1, string path2) =>
        Path.Combine(path1, path2);

    #endregion

    #region Registry

    /// <summary>
    /// Register file type (Windows only)
    /// </summary>
    /// <param name="fileTypeName">File type name (e.g. application)</param>
    /// <param name="fileTypeExtension">File type extension (e.g. .txt)</param>
    /// <param name="executable">The executable path</param>
    /// <remarks>see https://stackoverflow.com/a/46900103</remarks>>
    public static int RegisterFileType(string fileTypeName, string fileTypeExtension, string executable)
    {
        if (string.IsNullOrWhiteSpace(fileTypeName))
        {
            throw new ArgumentException(nameof(fileTypeName));
        }
        if (string.IsNullOrWhiteSpace(fileTypeExtension))
        {
            throw new ArgumentException(nameof(fileTypeExtension));
        }
        if (string.IsNullOrWhiteSpace(executable))
        {
            throw new ArgumentException(nameof(executable));
        }
        if (!FileExists(executable))
        {
            throw new ArgumentException($"Missing executable {executable}", nameof(executable));
        }

        if (!IsWindows())
        {
            return -1;
        }

        // file type
        var result = ExecuteProcess(
            fileName: "cmd",
            arguments: $"/c ftype {fileTypeName}={executable} \"%1\"");
        if (result != 0)
        {
            return result;
        }

        // association
        result = ExecuteProcess(
            fileName: "cmd",
            arguments: $"/c assoc {fileTypeExtension}={fileTypeName}");
        return result;
    }

    /// <summary>
    /// Unregister file type (Windows only)
    /// </summary>
    /// <param name="fileTypeName">File type name (e.g. application)</param>
    /// <param name="fileTypeExtension">File type extension (e.g. .txt)</param>
    public static int UnregisterFileType(string fileTypeName, string fileTypeExtension)
    {
        if (string.IsNullOrWhiteSpace(fileTypeName))
        {
            throw new ArgumentException(nameof(fileTypeName));
        }
        if (string.IsNullOrWhiteSpace(fileTypeExtension))
        {
            throw new ArgumentException(nameof(fileTypeExtension));
        }

        if (!IsWindows())
        {
            return -1;
        }

        // association
        // see https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-R2-and-2012/cc770920(v=ws.11)
        var result = ExecuteProcess(
            fileName: "cmd",
            arguments: $"/c assoc {fileTypeExtension}= ");
        if (result != 0)
        {
            return result;
        }

        // file type
        // see https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-R2-and-2012/cc771394(v=ws.11)
        result = ExecuteProcess(
            fileName: "cmd",
            arguments: $"/c ftype {fileTypeName}=");
        return result;
    }

    #endregion

    #region Process

    /// <summary>
    /// Start process
    /// </summary>
    /// <param name="fileName">Process name or the associated file</param>
    public static void StartProcess(string fileName)
    {
        // open file explorer
        var psi = new ProcessStartInfo(fileName)
        {
            UseShellExecute = !IsLinux()
        };
        using var process = Process.Start(psi);
    }

    /// <summary>
    /// Start process
    /// </summary>
    /// <param name="fileName">Process name or the associated file</param>
    /// <param name="arguments">Process arguments</param>
    public static void StartProcess(string fileName, string arguments)
    {
        var info = new ProcessStartInfo(fileName, arguments)
        {
            UseShellExecute = !IsLinux()
        };
        using var process = Process.Start(info);
    }

    /// <summary>
    /// Execute process and wait for the process result
    /// </summary>
    /// <param name="fileName">Process name or the associated file</param>
    /// <param name="arguments">Process arguments</param>
    /// <param name="windowStyle">Process window style</param>
    private static int ExecuteProcess(string fileName, string arguments,
        ProcessWindowStyle windowStyle = ProcessWindowStyle.Minimized)
    {
        using var process = new Process();
        process.StartInfo = new()
        {
            FileName = fileName,
            Arguments = arguments,
            WindowStyle = windowStyle,
            UseShellExecute = !IsLinux()
        };
        process.Start();
        process.WaitForExit();
        var exitCode = process.ExitCode;
        return exitCode;
    }

    /// <summary>
    /// Start a dot net webserver
    /// </summary>
    /// <param name="workingDirectory">Webserver path</param>
    /// <param name="webserverExec">Executable file name</param>
    /// <param name="webserverUrl">Target webserver url</param>
    /// <param name="webserverName">Server name</param>
    public static void StartWebserver(string workingDirectory, string webserverExec,
        string webserverUrl, string webserverName)
    {
        Directory.SetCurrentDirectory(workingDirectory);

        var fileName = "dotnet.exe";
        var arguments = $"{webserverExec} --urls={webserverUrl}";
        if (IsWindows() && !string.IsNullOrWhiteSpace(webserverName))
        {
            fileName = "cmd.exe";
            arguments = $"/C START /MIN \"Payroll Engine - Backend Server\" dotnet {webserverExec} --urls={webserverUrl}";
        }
        var info = new ProcessStartInfo(fileName, arguments)
        {

            WorkingDirectory = workingDirectory,
            UseShellExecute = !IsLinux(),
            WindowStyle = ProcessWindowStyle.Minimized
        };

        using var process = Process.Start(info);
    }

    #endregion

    #region Scurity

    /// <summary>
    /// Test for local dot net https developer certificate
    /// </summary>
    public static bool HasLocalSecureDevCertificate() =>
        ExecuteProcess(
            fileName: "dotnet.exe",
            arguments: "dev-certs https --check --trust",
            windowStyle: ProcessWindowStyle.Minimized) == 0;

    /// <summary>
    /// Add local dot net https developer certificate
    /// </summary>
    public static void AddLocalSecureDevCertificate() =>
        ExecuteProcess(
            fileName: "dotnet.exe",
            arguments: "dev-certs https --trust",
            windowStyle: ProcessWindowStyle.Minimized);

    /// <summary>
    /// Test for admin rights
    /// </summary>
    public static bool IsAdministrator()
    {
        if (!System.OperatingSystem.IsWindows())
        {
            return false;
        }

        // windows admin
        using var identity = WindowsIdentity.GetCurrent();
        var principal = new WindowsPrincipal(identity);
        var admin = principal.IsInRole(WindowsBuiltInRole.Administrator);
        return admin;
    }

    #endregion

}