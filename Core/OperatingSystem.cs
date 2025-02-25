using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace PayrollEngine.AdminApp;

/// <summary>
/// Operating system abstraction
/// </summary>
public static class OperatingSystem
{

    #region OS

    /// <summary>
    /// Test for linux OS
    /// </summary>
    private static bool IsLinux() =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

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
    private static string GetCurrentDirectory() =>
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
    /// <param name="webserverName">Server name (currently not supported)</param>
    /// <param name="environment">Process environment</param>
    public static void StartWebserver(string workingDirectory, string webserverExec,
        // ReSharper disable once UnusedParameter.Global
        string webserverUrl, string webserverName,Dictionary<string, string> environment = null)
    {
        Directory.SetCurrentDirectory(workingDirectory);

        var arguments = $"{webserverExec} --urls={webserverUrl}";
        var info = new ProcessStartInfo("dotnet.exe", arguments)
        {

            WorkingDirectory = workingDirectory,
            UseShellExecute = false,
            //UseShellExecute = !IsLinux(),
            WindowStyle = ProcessWindowStyle.Minimized
        };
        if (environment != null)
        {
            foreach (var item in environment)
            {
                info.Environment.Add(item);
            }
        }

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

    #endregion

}