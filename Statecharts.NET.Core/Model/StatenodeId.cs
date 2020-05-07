using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public class StatenodeId : IEquatable<StatenodeId>
    {
        public IEnumerable<string> Values { get; }
        internal string String => string.Join(".", Values);

        public StatenodeId(Option<Statenode> parent, string statenodeName) =>
            Values = parent.Match(
                parentStatenode => parentStatenode.Id.Values.Append(statenodeName),
                statenodeName.Yield);
        private StatenodeId(IEnumerable<string> values) =>
            Values = values;

        internal static StatenodeId Root(string name) =>
            new StatenodeId(Option.None<Statenode>(), name);
        internal static StatenodeId DeriveFromParent(Statenode parent, string name) =>
            new StatenodeId(parent.ToOption(), name);

        public bool Equals(StatenodeId other) => other != null && other.String.Equals(String);
        public override bool Equals(object obj) =>
            ReferenceEquals(this, obj) || !(obj is null) && obj is StatenodeId other && Equals(other);

        public override int GetHashCode() => String.GetHashCode();

        public override string ToString() => String;

        public StatenodeId Sibling(string siblingStatenodeName, params string[] childStatenodeNames) // TODO: think of rootState
            => new StatenodeId(Values.Take(Values.Count() - 1).Concat(siblingStatenodeName.Append(childStatenodeNames)));
        public StatenodeId Child(string childStatenodeName, params string[] childStatenodeNames) // TODO: think of rootState
            => new StatenodeId(Values.Append(childStatenodeName).Concat(childStatenodeNames));
        public static StatenodeId Absolute(IEnumerable<string> statenodeids)
            => new StatenodeId(statenodeids); // TODO: check this
    }
}
