using System;
using Statecharts.NET.Definition;

namespace Statecharts.NET.Interpreter
{
    internal class AtomicStateNode<TContext> : BaseStateNode<TContext>
        where TContext : IEquatable<TContext>
    {
        public AtomicStateNode(BaseStateNode<TContext> parent, AtomicStateNodeDefinition<TContext> definition) : base(parent, definition) { }
    }
}
