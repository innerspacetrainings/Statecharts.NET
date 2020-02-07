using System;
using System.Collections.Generic;
using System.Text;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.Definition
{
    internal class ContextDataEntry
    {
        internal string Id { get; set; }
        internal Option<string> ValueExpression { get; set; } = Option.None<string>();
    }
}
