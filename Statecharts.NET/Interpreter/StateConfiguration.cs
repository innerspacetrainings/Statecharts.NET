using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Statecharts.NET.Interpreter;

namespace Statecharts.NET
{
    public static class StateConfigurationFunctions
    {
        public static StateConfiguration Without(this StateConfiguration stateConfiguration, IEnumerable<StateNodeId> stateNodeIds)
            => new StateConfiguration(stateConfiguration.StateNodeIds.Except(stateNodeIds));
        public static StateConfiguration Without(this StateConfiguration stateConfiguration, StateNodeId stateNodeId)
            => stateConfiguration.Without(new[] { stateNodeId });
        public static StateConfiguration With(this StateConfiguration stateConfiguration, IEnumerable<StateNodeId> stateNodeIds)
            => new StateConfiguration(stateConfiguration.StateNodeIds.Concat(stateNodeIds));
        public static StateConfiguration With(this StateConfiguration stateConfiguration, StateNodeId stateNodeId)
            => stateConfiguration.With(new[] {stateNodeId});
        public static bool Contains(this StateConfiguration stateConfiguration, StateNode stateNode)
            => stateConfiguration.StateNodeIds.Contains(stateNode.Id);
    }

    public class StateConfiguration
    {
        public static StateConfiguration NotInitialized => new StateConfiguration(Enumerable.Empty<StateNodeId>());

        public StateConfiguration(IEnumerable<StateNodeId> stateIds)
        {
            StateNodeIds = stateIds ?? throw new ArgumentNullException(nameof(stateIds));
        }

        public IEnumerable<StateNodeId> StateNodeIds { get; }

        public bool IsNotInitialized => !StateNodeIds.Any();
    }
}
