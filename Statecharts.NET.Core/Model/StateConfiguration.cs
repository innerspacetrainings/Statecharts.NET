using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public class StateConfiguration : IEnumerable<StatenodeId>
    {
        private static IEnumerable<StatenodeId> Ids(IEnumerable<Statenode> statenodes)
            => statenodes?.Select(statenode => statenode.Id);

        public IList<StatenodeId> StateNodeIds { get; }

        public IEnumerable<string> NonRootIds =>
            StateNodeIds
                .Select(id => id.Values.Skip(1))
                .Where(values => values.Any())
                .Select(values => string.Join(".", values));

        private StateConfiguration(IEnumerable<StatenodeId> statenodeIds) =>
            StateNodeIds = statenodeIds?.ToList() ?? throw new ArgumentNullException(nameof(statenodeIds));
        public StateConfiguration(IEnumerable<Statenode> statenodes) : this (Ids(statenodes)) { }
        public static StateConfiguration Empty() => new StateConfiguration(Enumerable.Empty<StatenodeId>());

        public bool Contains(Statenode statenode)
            => StateNodeIds.Contains(statenode.Id);

        public IEnumerator<StatenodeId> GetEnumerator() => StateNodeIds.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public StateConfiguration Without(IEnumerable<Statenode> statenodes) =>
            new StateConfiguration(StateNodeIds.Except(Ids(statenodes)));
        public StateConfiguration Without(Statenode statenode) =>
            Without(statenode.Yield());
        public StateConfiguration With(IEnumerable<Statenode> statenodes) =>
            new StateConfiguration(StateNodeIds.Concat(Ids(statenodes)));
        public StateConfiguration With(Statenode statenode) =>
            With(statenode.Yield());
    }
}
