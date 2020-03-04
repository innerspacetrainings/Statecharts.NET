using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Statecharts.NET.Model
{
    // TODO
    public static class StateConfigurationFunctions
    {
        public static StateConfiguration Without(this StateConfiguration stateConfiguration, IEnumerable<StateNodeId> stateNodeIds)
            => new StateConfiguration(stateConfiguration.StateNodeIds.Except(stateNodeIds));
        public static StateConfiguration Without(this StateConfiguration stateConfiguration, StateNodeId stateNodeId)
            => stateConfiguration.Without(new[] { stateNodeId });
        public static StateConfiguration With(this StateConfiguration stateConfiguration, IEnumerable<StateNodeId> stateNodeIds)
            => new StateConfiguration(stateConfiguration.StateNodeIds.Concat(stateNodeIds));
        public static StateConfiguration With(this StateConfiguration stateConfiguration, StateNodeId stateNodeId)
            => stateConfiguration.With(new[] { stateNodeId });
        public static bool Contains(this StateConfiguration stateConfiguration, StateNode stateNode)
            => stateConfiguration.StateNodeIds.Contains(stateNode.Id);
    }

    public class StateConfiguration : IEnumerable<StatenodeId>
    {
        public IEnumerable<StatenodeId> StateNodeIds { get; }

        public StateConfiguration(IEnumerable<Statenode> statenodes) =>
            StateNodeIds = statenodes?.Select(statenode => statenode.Id) ?? throw new ArgumentNullException(nameof(statenodes));

        public bool Contains(Statenode statenode)
            => StateNodeIds.Contains(statenode.Id);

        public IEnumerator<StatenodeId> GetEnumerator() => StateNodeIds.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
