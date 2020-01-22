using System;

namespace Statecharts.NET.Language.Statechart
{
    public class Builder
    {
        public Builder<TContext> WithInitialContext<TContext>(TContext initialContext)
            where TContext : IEquatable<TContext>
            => new Builder<TContext>(initialContext);
    }

    public class Builder<TContext>
        where TContext : IEquatable<TContext>
    {
        private readonly TContext _initialContext;

        internal Builder(TContext initialContext)
            => _initialContext = initialContext;

        public Definition.Statechart<TContext> WithRootState(Definition.CompoundStateNode rootStateNode)
            => new Definition.Statechart<TContext>(_initialContext, rootStateNode);

        public Definition.Statechart<TContext> WithRootState(Definition.OrthogonalStateNode rootStateNode)
            => new Definition.Statechart<TContext>(_initialContext, rootStateNode);
    }
}
