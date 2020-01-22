using System;
using System.Collections.Generic;
using System.Text;

namespace Statecharts.NET
{
    public class State<TContext>
        where TContext : IEquatable<TContext>
    {
        public StateConfiguration StateConfiguration { get; }
        public TContext Context { get; }

        public State(StateConfiguration stateConfiguration, TContext context)
        {
            StateConfiguration = stateConfiguration ?? throw new ArgumentNullException(nameof(stateConfiguration));
            Context = context;
        }
    }
}
