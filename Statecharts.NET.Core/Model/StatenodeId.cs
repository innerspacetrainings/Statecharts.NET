using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool Equals(StatenodeId other) => other != null && other.String.Equals(String);
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || !(obj is null) && obj is StatenodeId other && Equals(other);

        public override int GetHashCode() => String.GetHashCode();

        public StatenodeId Sibling(string siblingStatenodeName) // TODO: think of rootState
            => new NamedStatenodeId(Values.Take(Values.Count() - 1).Append(siblingStatenodeName));
        public StatenodeId Child(string childStatenodeName) // TODO: think of rootState
            => new NamedStatenodeId(Values.Append(childStatenodeName));
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
        internal NamedStatenodeId(IEnumerable<string> values) =>
            Values = values;
    }
}
