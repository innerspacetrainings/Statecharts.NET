using System;
using System.Threading.Tasks;

namespace Statecharts.NET.Interpreter
{
    // TODO: probably implement a custom awaiter, so this can directly be awaited (https://stackoverflow.com/a/9265496, @Jon Skeet)
    public class StartResult<TContext> where TContext : IEquatable<TContext>
    {
        public StartResult(State<TContext> state, Task<object> task)
        {
            State = state;
            Task = task;
        }

        public State<TContext> State { get; }
        public Task Task { get; }
    }
}
