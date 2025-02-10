using System;
using System.Linq;
using System.Collections.Generic;

namespace PayrollEngine.AdminApp.Asset;

/// <summary>
/// Database asset parameter
/// </summary>
public class DatabaseAssetParameter : IAssetParameter
{
    // used for json serialization
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    /// <summary>
    /// Database minimum version
    /// </summary>
    public Version MinVersion { get; set; }

    /// <summary>
    /// Database current version
    /// </summary>
    public Version CurrentVersion { get; set; }
    // ReSharper restore UnusedAutoPropertyAccessor.Global

    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
    // ReSharper disable CollectionNeverUpdated.Global
    /// <summary>
    /// Setup initialize scripts
    /// </summary>
    public List<string> InitScripts { get; set; } = [];

    /// <summary>
    /// Setup update scripts
    /// </summary>
    public List<DatabaseUpdateAssetParameter> UpdateScripts { get; set; } = [];
    // ReSharper restore CollectionNeverUpdated.Global
    // ReSharper restore AutoPropertyCanBeMadeGetOnly.Global

    /// <inheritdoc />
    public void Validate()
    {
        if (MinVersion.IsEmpty())
        {
            throw new AdminException("Missing database min version parameter.");
        }
        if (CurrentVersion.IsEmpty())
        {
            throw new AdminException("Missing database current version parameter.");
        }

        if (MinVersion > CurrentVersion)
        {
            throw new AdminException("Database parameter min version is invalid.");
        }

        if (!InitScripts.Any())
        {
            throw new AdminException("Missing database init scripts parameter.");
        }

        if (!MinVersion.Equals(CurrentVersion) && !UpdateScripts.Any())
        {
            throw new AdminException("Missing database update scripts parameter.");
        }

        if (MinVersion.Equals(CurrentVersion) && UpdateScripts.Any())
        {
            throw new AdminException($"Invalid update script on initial database version {CurrentVersion}.");
        }

        if (!UpdateScripts.Any())
        {
            return;
        }

        // validate update script
        foreach (var updateScript in UpdateScripts)
        {
            updateScript.Validate();
        }

        if (UpdateScripts.Count == 1)
        {
            return;
        }

        // first version should be the min-version
        var firstVersion = UpdateScripts.First().FromVersion;
        if (!firstVersion.Equals(MinVersion))
        {
            throw new AdminException($"First update version ({firstVersion}) does not match the minimum version ({MinVersion}).");
        }

        // validate closed version chain
        var updatesByVersion = UpdateScripts.OrderBy(x => x.FromVersion).ToList();
        for (var i = 0; i < updatesByVersion.Count - 1; i++)
        {
            var curVersion = updatesByVersion[i];
            var nextVersion = updatesByVersion[i + 1];
            if (!curVersion.ToVersion.Equals(nextVersion.FromVersion))
            {
                throw new AdminException($"Version update gap between version {curVersion.ToVersion} and {nextVersion.ToVersion}.");
            }
        }

        // last version should be the current version
        var lastVersion = UpdateScripts.Last().ToVersion;
        if (!lastVersion.Equals(CurrentVersion))
        {
            throw new AdminException($"Last update version ({lastVersion}) does not match the current version ({CurrentVersion}).");
        }
    }
}