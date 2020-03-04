using System;
using System.Collections.Generic;
using System.Text;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public class StatenodeId : OneOfBase<RootStatenodeId, NamedStatenodeId>, IEquatable<StatenodeId>
    {
        private StatenodeId() { }
        internal static StatenodeId Parent(string name) =>
        internal static StatenodeId DeriveFromParent(Statenode parent, string name) =>
            new NamedStatenodeId(parent, name);
    }

    public class RootStatenodeId { }
    public class NamedStatenodeId { }
}
