using System;

namespace PayrollEngine.AdminApp;

/// <summary>
/// Extension methods for <see cref="Version"/>
/// </summary>
public static class VersionExtensions
{
    /// <summary>
    /// Test for empty version
    /// </summary>
    /// <param name="version">Version to test</param>
    public static bool IsEmpty(this Version version) =>
        version == null || version.Major == 0 && version.Minor == 0;
}