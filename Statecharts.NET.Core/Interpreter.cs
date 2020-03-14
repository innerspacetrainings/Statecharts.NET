using System.Threading;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;

namespace Statecharts.NET
{
    public class Interpreter
    {
        public static RunningStatechart<TContext> Interpret<TContext>(ExecutableStatechart<TContext> statechart) // options e.g. Logger
            where TContext : IContext<TContext> =>
            Interpret(statechart, CancellationToken.None);
        public static RunningStatechart<TContext> Interpret<TContext>(ExecutableStatechart<TContext> statechart, CancellationToken cancellationToken) // options e.g. Logger
            where TContext : IContext<TContext> =>
            new RunningStatechart<TContext>(statechart, cancellationToken);
    }
}
