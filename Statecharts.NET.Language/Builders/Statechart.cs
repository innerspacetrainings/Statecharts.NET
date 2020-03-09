using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;

namespace Statecharts.NET.Language.Builders
{
    public class Statechart<TContext>
        where TContext : IContext<TContext>
    {
        private readonly TContext _initialContext;

        internal Statechart(TContext initialContext)
            => _initialContext = initialContext;

        public StatechartDefinition<TContext> WithRootState(CompoundStatenodeDefinition rootStateNode)
            => new StatechartDefinition<TContext>(_initialContext, rootStateNode);

        public StatechartDefinition<TContext> WithRootState(OrthogonalStatenodeDefinition rootStateNode)
            => new StatechartDefinition<TContext>(_initialContext, rootStateNode);
    }
}
