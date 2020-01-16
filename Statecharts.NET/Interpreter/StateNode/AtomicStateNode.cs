using System;
using Statecharts.NET.Definition;

namespace Statecharts.NET.Interpreter
{
    internal class AtomicStateNode<TContext> : BaseStateNode<TContext>
        where TContext : IEquatable<TContext>
    {
        public AtomicStateNode(BaseStateNode<TContext> parent, IAtomicStateNodeDefinition definition) : base(parent, definition) { }
    }
}
