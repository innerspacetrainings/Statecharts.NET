using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public class StateConfiguration
    {
        private readonly IDictionary<StatenodeId, Entry> _lookup = new Dictionary<StatenodeId, Entry>();
        private Entry _root = null;

        public class Entry
        {
            public StatenodeId StatenodeId { get; }
            public string StatenodeName { get; }
            public ISet<Entry> Children { get; set; }

            public Entry(StatenodeId statenodeId, string statenodeName)
            {
                StatenodeId = statenodeId ?? throw new NullReferenceException(nameof(statenodeId));
                StatenodeName = statenodeName;
                Children = new HashSet<Entry>();
            }

            private bool Equals(Entry other) => Equals(StatenodeId, other.StatenodeId);
            public override bool Equals(object obj) => obj != null && obj.GetType() == GetType() && Equals((Entry) obj);
            public override int GetHashCode() => StatenodeId.GetHashCode();
        }

        public Entry Root => _root;

        internal void Add(ParsedStatenode statenode)
        {
            var entry = new Entry(statenode.Id, statenode.Name);
            statenode.Parent.Switch(
                parent => _lookup[parent.Id].Children.Add(entry),
                () => _root = entry);
            _lookup.Add(statenode.Id, entry);
        }

        internal void Remove(ParsedStatenode statenode)
        {
            var entry = new Entry(statenode.Id, statenode.Name);
            statenode.Parent.Switch(
                parent => _lookup[parent.Id].Children.Remove(entry),
                () => _root = null);
            _lookup.Remove(statenode.Id);
        }

        public bool Contains(ParsedStatenode statenode)
            => _lookup.ContainsKey(statenode.Id);

        public bool IsInitialized(ParsedStatenode statenode)
            => Contains(statenode) && _lookup[statenode.Id].Children.Any();

        public IEnumerable<Entry> FoldL()
        {
            var entries = new List<Entry>();
            void Recurse(Entry entry)
            {
                foreach (var child in entry.Children)
                    Recurse(child);
                entries.Add(entry);
            }

            _root.ToOption().SwitchSome(Recurse);

            return entries;
        }

        public IEnumerable<StatenodeId> Ids
            => _lookup.Values.Select(entry => entry.StatenodeId);

        public static StateConfiguration Empty() => new StateConfiguration();


        ////public StateConfiguration Without(IEnumerable<Statenode> statenodes) =>
        ////    new StateConfiguration(StateNodeIds.Except(Ids(statenodes)));
        ////public StateConfiguration Without(Statenode statenode) =>
        ////    Without(statenode.Yield());
        ////public StateConfiguration With(IEnumerable<Statenode> statenodes) =>
        ////    new StateConfiguration(StateNodeIds.Concat(Ids(statenodes)));
        ////public StateConfiguration With(Statenode statenode) =>
        ////    With(statenode.Yield());
    }
}
