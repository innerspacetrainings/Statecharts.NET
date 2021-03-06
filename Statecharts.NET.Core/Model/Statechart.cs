﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        where TContext : IContext<TContext>
    {
        public ParsedStatenode Rootnode { get; }

        public string Id => Rootnode.Name;

        protected ParsedStatechart(ParsedStatenode rootnode) =>
            Rootnode = rootnode ?? throw new ArgumentNullException(nameof(rootnode));
    }

    class InvalidStatechart<TContext> : ParsedStatechart<TContext> where TContext : IContext<TContext>
    {
        public InvalidStatechart() : base(null) =>
            throw new NotImplementedException();
    }

    class ValidStatechart<TContext> : ParsedStatechart<TContext> where TContext : IContext<TContext>
    {
        public ValidStatechart() : base(null) =>
            throw new NotImplementedException();
    }

    public class ExecutableStatechart<TContext> : ParsedStatechart<TContext> where TContext : IContext<TContext>
    {
        internal Action<TContext, object> Done { get; set; }
        public TContext InitialContext { get; }
        public IDictionary<StatenodeId, ParsedStatenode> Statenodes { get; }

        public ExecutableStatechart(ParsedStatenode rootnode, TContext initialContext, IDictionary<StatenodeId, ParsedStatenode> statenodes) : base(rootnode)
        {
            InitialContext = initialContext;
            Statenodes = statenodes;
        }

        public IEnumerable<ParsedStatenode> GetActiveStatenodes(StateConfiguration stateConfiguration) => 
            stateConfiguration.FoldL().Select(entry => Statenodes[entry.StatenodeId]);
    }
    #endregion
}
