using System;

namespace Statecharts.NET.Language.Builders
{
    public class Statechart<TContext>
        where TContext : IEquatable<TContext>
    {
        private readonly TContext _initialContext;

        internal Statechart(TContext initialContext)
            => _initialContext = initialContext;

        public Definition.Statechart<TContext> WithRootState(Definition.CompoundStateNode rootStateNode)
            => new Definition.Statechart<TContext>(_initialContext, rootStateNode);

        public Definition.Statechart<TContext> WithRootState(Definition.OrthogonalStateNode rootStateNode)
            => new Definition.Statechart<TContext>(_initialContext, rootStateNode);
    }
}
