using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;

namespace Statecharts.NET
{
    public class Parser
    {
        private static Statenode ParseStatenode(StatenodeDefinition definition, Statenode parent)
        {
            definition.Match(
                atomic => new AtomicStatenode(),
                final => new FinalStatenode(),
                compound => new CompoundStatenode(), 
                orthogonal => new OrthogonalStatenode());
            new CompoundStatenode(Transform(definition.InitialTransition))
        }

        public ParsedStatechart<TContext> Parse<TContext>(StatechartDefinition<TContext> definition)
            where TContext : IContext<TContext> =>
            // TODO: return actual ParsedStatechart based on results from parsing
            new Model.ExecutableStatechart<TContext>(
                ParseStatenode(definition.RootStateNode, null),
                definition.InitialContext.CopyDeep());
    }
}
