using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;

namespace Statecharts.NET.Interpreter
{
    class OrthogonalStateNode<TContext> : BaseStateNode<TContext>
        where TContext : IEquatable<TContext>
    {
        public IEnumerable<BaseStateNode<TContext>> StateNodes { get; internal set; }

        public OrthogonalStateNode(
            BaseStateNode<TContext> parent,
            OrthogonalStateNodeDefinition<TContext> definition) : base(parent, definition)
        {
        }

        public BaseStateNode<TContext> GetSubstate(NamedStateNodeKey key)
            => StateNodes.FirstOrDefault(state => state.Key == key) ?? throw new Exception("WTF is happening");
    }
}
