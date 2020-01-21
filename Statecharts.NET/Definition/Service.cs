using System;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public abstract class Service : OneOfBase<TaskService, TaskDataService>
    {
        string Id { get; }
        UnguardedTransition OnErrorTransition { get; }
    }

    public class TaskService : Service
    {
        Func<CancellationToken, Task> Task { get; }
        UnguardedTransition OnSuccessDefinition { get; }
    }

    public class TaskDataService : Service
    {
        Func<CancellationToken, Task<object>> Task { get; }
        UnguardedContextDataTransition OnSuccessDefinition { get; }
    }
}
