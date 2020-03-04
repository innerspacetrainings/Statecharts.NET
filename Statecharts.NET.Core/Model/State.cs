using System;
using Statecharts.NET.Interfaces;

namespace Statecharts.NET.Model
{
    public class State<TContext>
        where TContext : IContext<TContext>
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
