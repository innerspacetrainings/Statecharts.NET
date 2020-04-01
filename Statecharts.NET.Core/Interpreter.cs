using System;
using System.Threading;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;

namespace Statecharts.NET
{
    public class InterpreterOptions
    {
        public Action<string> Log { get; set; }
        public Func<TimeSpan, CancellationToken, System.Threading.Tasks.Task> Wait { get; set; }

        public InterpreterOptions(Action<string> log = null, Func<TimeSpan, CancellationToken, System.Threading.Tasks.Task> wait = null)
        {
            Log = log ?? Console.WriteLine;
            Wait = wait ?? System.Threading.Tasks.Task.Delay;
        }
    }

    public class Interpreter
    {
        public static RunningStatechart<TContext> Interpret<TContext>(ExecutableStatechart<TContext> statechart, InterpreterOptions options = null) // options e.g. Logger
            where TContext : IContext<TContext> =>
            Interpret(statechart, CancellationToken.None, options);
        public static RunningStatechart<TContext> Interpret<TContext>(ExecutableStatechart<TContext> statechart, CancellationToken cancellationToken, InterpreterOptions options = null) // options e.g. Logger
            where TContext : IContext<TContext> =>
            new RunningStatechart<TContext>(statechart, cancellationToken, options ?? new InterpreterOptions());
    }
}
