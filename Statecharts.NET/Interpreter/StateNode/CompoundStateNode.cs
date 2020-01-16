using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;

namespace Statecharts.NET.Interpreter
{
    class CompoundStateNode<TContext> : BaseStateNode<TContext>
        where TContext : IEquatable<TContext>
    {
        public InitialTransition<TContext> InitialTransition { get; internal set; }

        public IEnumerable<BaseStateNode<TContext>> StateNodes { get; internal set; }

        public CompoundStateNode(
            BaseStateNode<TContext> parent,
            ICompoundStateNodeDefinition definition) : base(parent, definition)
        {
        }

        public BaseStateNode<TContext> GetSubstate(NamedStateNodeKey key)
            => StateNodes.FirstOrDefault(state => state.Key.Equals(key)) ?? throw new Exception("WTF is happening");
    }
}
