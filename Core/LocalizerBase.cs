﻿using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Localization;

namespace PayrollEngine.AdminApp;

/// <summary>
/// Localizer (copy from PayrollEngine.WebApp.Shared.LocalizerBase)
/// </summary>
public abstract class LocalizerBase
{
    private IStringLocalizerFactory Factory { get; }
    private IStringLocalizer Localizer { get; }
    private string GroupName { get; }

    private const string ResourceName = "Localizations";

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="factory">String localizer factory</param>
    /// <param name="groupName">Additional group name</param>
    protected LocalizerBase(IStringLocalizerFactory factory, string groupName = null)
    {
        Factory = factory ?? throw new ArgumentNullException(nameof(factory));

        // localization group: <GroupName>.<LocalizationKey>
        if (string.IsNullOrWhiteSpace(groupName))
        {
            var typePostfix = nameof(Localizer);
            groupName = GetType().Name;
            if (groupName.EndsWith(typePostfix))
            {
                groupName = groupName.Substring(0, groupName.Length - typePostfix.Length);
            }
        }
        GroupName = groupName;

        // string localizer
        var assemblyName = GetType().Assembly.FullName;
        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            throw new ArgumentException(nameof(assemblyName));
        }
        Localizer = Factory.Create(ResourceName, assemblyName);
    }

    /// <summary>
    /// Retrieve translation from group and key
    /// Translation example: MyGroup.MyKey
    /// </summary>
    /// <param name="group">The localization group</param>
    /// <param name="key">The localization key</param>
    private string Key(string group, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(nameof(key));
        }
        return string.IsNullOrWhiteSpace(group) ? Localizer[key] : Localizer[$"{group}.{key}"];
    }

    /// <summary>
    /// Retrieve translation from current group and key
    /// Resource name: {InternalGroupName}.MyText
    /// </summary>
    /// <param name="key">The localization key</param>
    private string Key(string key) =>
        Key(GroupName, key);

    /// <summary>
    /// Retrieve translation from group key, group and key are identical
    /// Resource name: MyText.MyText
    /// </summary>
    /// <param name="groupAndKey">The group key</param>
    public string GroupKey(string groupAndKey) =>
        Key(groupAndKey, groupAndKey);

    /// <summary>
    /// Retrieve translation value from property key
    /// Resource name: MyProperty.MyText
    /// </summary>
    /// <param name="caller">The caller method name</param>
    protected string PropertyValue([CallerMemberName] string caller = null) =>
        Key(caller);

    /// <summary>
    /// Format translation value with one parameter
    /// </summary>
    /// <param name="format">The value to format</param>
    /// <param name="parameterName">The parameter name</param>
    /// <param name="parameterValue">The parameter value</param>
    protected string FormatValue(string format, string parameterName, object parameterValue) =>
        format.Replace($"{{{parameterName}}}", parameterValue?.ToString());

    /// <summary>
    /// Format translation value with two parameter
    /// </summary>
    /// <param name="format">The value to format</param>
    /// <param name="firstParameterName">The first parameter name</param>
    /// <param name="firstParameterValue">The first parameter value</param>
    /// <param name="secondParameterName">The second parameter name</param>
    /// <param name="secondParameterValue">The second parameter value</param>
    protected string FormatValue(string format, string firstParameterName, object firstParameterValue,
        string secondParameterName, object secondParameterValue)
    {
        format = FormatValue(format, firstParameterName, firstParameterValue);
        return FormatValue(format, secondParameterName, secondParameterValue);
    }

    /// <summary>
    /// Retrieve translation from enum value
    /// Resource name: Enum.{EnumTypeName}.MyEnumValue
    /// </summary>
    /// <param name="value">The enum value</param>
    /// <param name="mapType">Enum type mapping</param>
    public string Enum<T>(T value, string mapType = null)
    {
        var type = typeof(T);

        // nullable enum
        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
        {
            type = nullableType;
        }

        var group = mapType ?? type.Name;
        var translation = Key(group, $"{value}");
        // missing translation: return the enum value
        if (string.IsNullOrEmpty(translation) || translation.StartsWith(group))
        {
            throw new AdminException($"Missing translation for enum type {group} with value {value}");
        }
        return translation;
    }
}