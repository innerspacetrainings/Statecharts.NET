using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Statecharts.NET.Analyzers
{
    internal static class Helpers
    {
        private static LocalizableString GetLocalizedString(string id, string type) =>
            new LocalizableResourceString($"{id}.{type}", Resources.ResourceManager, typeof(Resources));

        internal static DiagnosticDescriptor CreateRule(string id, string category, DiagnosticSeverity severity, bool isEnabledByDefault) =>
            new DiagnosticDescriptor(
                id,
                GetLocalizedString(id, "Title"),
                GetLocalizedString(id, "MessageFormat"),
                category,
                severity,
                isEnabledByDefault,
                GetLocalizedString(id, "Description"));
    }
}
