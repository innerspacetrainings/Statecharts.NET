using System;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public abstract class Service : OneOfBase<TaskService, TaskService>
    {
        string Id { get; }
        UnguardedTransition OnErrorTransition { get; }
    }

    public class TaskService : Service
    {
        Func<CancellationToken, Task> Task { get; }
        UnguardedTransition OnSuccessDefinition { get; }
    }

    public class TaskService<> : Service where TContext : IEquatable<TContext>
    {
        Func<CancellationToken, Task<TResult>> Task { get; }
        UnguardedTransition<TContext, TResult> OnSuccessDefinition { get; }
    }
}
