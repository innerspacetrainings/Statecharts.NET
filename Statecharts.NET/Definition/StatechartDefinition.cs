using System;

namespace Statecharts.NET.Definition
{
    public class StatechartDefinition<TContext>
        where TContext : IEquatable<TContext>
    {
        public string Id => StateNodeDefinition.Name;
        public TContext InitialContext { get; set; }
        public BaseStateNodeDefinition<TContext> StateNodeDefinition { get; set; }
    }
}
