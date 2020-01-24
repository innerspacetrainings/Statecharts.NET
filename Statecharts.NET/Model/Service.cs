using System;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Definition;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
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

    public abstract class ObservableService
    {
        public abstract IObservable<string> Observable { get; }
        public abstract OneOf<UnguardedTransition, UnguardedContextTransition> OnSuccessDefinition { get; }
    }

    public abstract class ActivityService : Service
    {
        public Activity Activity { get; }
    }
}