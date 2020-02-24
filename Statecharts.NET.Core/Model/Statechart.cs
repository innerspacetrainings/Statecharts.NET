using System;
using System.Collections.Generic;
using Statecharts.NET.Interfaces;

namespace Statecharts.NET.Model
{
    #region Definition
    public class StatechartDefinition<TContext>
        where TContext : IContext<TContext>
    {
        public string Id => RootStateNode.Name;
        public TContext InitialContext { get; }
        public StatenodeDefinition RootStateNode { get; }

        public StatechartDefinition(TContext initialContext, StatenodeDefinition rootStateNode)
        {
            if (initialContext == null) throw new ArgumentNullException(nameof(initialContext));
            InitialContext = initialContext;
            RootStateNode = rootStateNode ?? throw new ArgumentNullException(nameof(rootStateNode));
        }
    }
    #endregion
    #region Parsed
    public abstract class ParsedStatechart<TContext>
        where TContext : IEquatable<TContext>
    {
        public Interpreter.StateNode RootNode { get; set; }

        public string Id => RootNode.Key.GetType().Name; // TODO: get real name here

        public ParsedStatechart(Interpreter.StateNode rootNode) =>
            RootNode = rootNode ?? throw new ArgumentNullException(nameof(rootNode));
    }

    class InvalidStatechart<TContext> : ParsedStatechart<TContext> where TContext : IEquatable<TContext>
    {
        public InvalidStatechart() : base(null) =>
            throw new NotImplementedException();
    }

    class ValidStatechart<TContext> : ParsedStatechart<TContext> where TContext : IEquatable<TContext>
    {
        public ValidStatechart() : base(null) =>
            throw new NotImplementedException();
    }
    #endregion
}
