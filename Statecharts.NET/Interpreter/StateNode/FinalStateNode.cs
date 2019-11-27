using System;
using Statecharts.NET.Definition;

namespace Statecharts.NET.Interpreter
{
    class FinalStateNode<TContext> : BaseStateNode<TContext>
        where TContext : IEquatable<TContext>
    {
        public FinalStateNode(BaseStateNode<TContext> parent, FinalStateNodeDefinition<TContext> definition) : base(parent, definition) { }
    }
}
