using System;
using System.Text;
using Microsoft.AspNetCore.Components;

namespace PayrollEngine.AdminApp.Presentation;

/// <summary>
/// Markup tool
/// </summary>
internal static class MarkupTool
{
    /// <summary>
    /// Build href markup
    /// </summary>
    /// <param name="url">Href ulr</param>
    /// <param name="text">Href text</param>
    /// <param name="cssStyle">Link css style</param>
    /// <param name="cssClass">Link css class</param>
    internal static MarkupString ToHref(string url, string text = null,
        string cssStyle = null, string cssClass = null)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            throw new ArgumentException(nameof(url));
        }

        text ??= url;

        var buffer = new StringBuilder();
        buffer.Append($"<a href=\"{url}\"");
        if (!string.IsNullOrWhiteSpace(cssStyle))
        {
            buffer.Append($" style=\"{cssStyle}\"");
        }
        if (!string.IsNullOrWhiteSpace(cssClass))
        {
            buffer.Append($" class=\"{cssClass}\"");
        }
        buffer.Append($">{text}</a>");

        return new(buffer.ToString());
    }
}