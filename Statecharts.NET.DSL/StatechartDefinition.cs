using System;
using System.Collections.Generic;
using System.Text;
using Statecharts.NET.Definition;

namespace Statecharts.NET.Builders
{
    public class StatechartDefinitionBuilder
    {
        public StatechartDefinitionBuilder<TContext> WithInitialContext<TContext>(TContext initialContext)
            where TContext : IEquatable<TContext>
            => new StatechartDefinitionBuilder<TContext>(initialContext);
    }

    public class StatechartDefinitionBuilder<TContext>
        where TContext : IEquatable<TContext>
    {
        private StatechartDefinition<TContext> Definition { get; }
        internal StatechartDefinitionBuilder(TContext initialContext)
            => Definition = new StatechartDefinition<TContext> { InitialContext = initialContext };

        public StatechartDefinition<TContext> WithRootState(ICompoundStateNodeDefinition stateNodeDefinition)
        {
            Definition.StateNodeDefinition = stateNodeDefinition;
            return Definition;
        }

        public StatechartDefinition<TContext> WithRootState(IOrthogonalStateNodeDefinition stateNodeDefinition)
        {
            Definition.StateNodeDefinition = stateNodeDefinition;
            return Definition;
        }
    }
}
