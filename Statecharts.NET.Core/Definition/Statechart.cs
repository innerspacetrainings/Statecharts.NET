using System;

namespace Statecharts.NET.Definition
{
    public class Statechart<TContext>
        where TContext : IEquatable<TContext>
    {
        public string Id => RootStateNode.Name;
        public TContext InitialContext { get; set; }
        public StateNode RootStateNode { get; set; }

        public Statechart(TContext initialContext, StateNode rootStateNode)
        {
            if (initialContext == null) throw new ArgumentNullException(nameof(initialContext));
            InitialContext = initialContext;
            RootStateNode = rootStateNode ?? throw new ArgumentNullException(nameof(rootStateNode));
        }
    }
}
