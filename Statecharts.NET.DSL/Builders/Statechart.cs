using System;
using Statecharts.NET.Definition;

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

        public Statechart<TContext> WithRootState(CompoundStateNode rootStateNode)
            => new Statechart<TContext>(_initialContext, rootStateNode);

        public Statechart<TContext> WithRootState(OrthogonalStateNode rootStateNode)
            => new Statechart<TContext>(_initialContext, rootStateNode);
    }
}
