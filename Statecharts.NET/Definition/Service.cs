using System;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public abstract class Service : OneOfBase<TaskService, TaskDataService>
    {
        public abstract string Id { get; }
        public abstract OneOf<UnguardedTransition, UnguardedContextTransition> OnErrorTransition { get; }
    }

    public abstract class TaskService : Service
    {
        public abstract Func<CancellationToken, Task> Task { get; }
        public abstract OneOf<UnguardedTransition, UnguardedContextTransition> OnSuccessDefinition { get; }
    }

    public abstract class TaskDataService : Service
    {
        public abstract Func<CancellationToken, Task<object>> Task { get; }
        public abstract OneOf<UnguardedTransition, UnguardedContextTransition, UnguardedContextTransition> OnSuccessDefinition { get; }
    }
}
