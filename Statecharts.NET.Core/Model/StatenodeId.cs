using System;
using System.Collections.Generic;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public abstract class StatenodeId : OneOfBase<RootStatenodeId, NamedStatenodeId>, IEquatable<StatenodeId>
    {
        internal abstract IEnumerable<string> Values { get; }
        internal string String => string.Join(".", Values);

        internal static StatenodeId Root(string name) =>
            new RootStatenodeId(name);
        internal static StatenodeId DeriveFromParent(Statenode parent, string name) =>
            new NamedStatenodeId(parent, name);

        public virtual bool Equals(StatenodeId other) =>
            Match(root => root.Equals(other), named => named.Equals(other));
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || !(obj is null) && obj is StatenodeId other && Equals(other);

        public override int GetHashCode() => String.GetHashCode();
    }

    public class RootStatenodeId : StatenodeId {
        public string StatenodeName { get; }
        internal override IEnumerable<string> Values => new[] {StatenodeName};
        public RootStatenodeId(string statenodeName) => StatenodeName = statenodeName;
    }
    public class NamedStatenodeId : StatenodeId
    {
        internal override IEnumerable<string> Values { get; }
        public NamedStatenodeId(Statenode parent, string name) =>
            Values = parent.Id.Values.Append(name);
    }
}
