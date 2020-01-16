using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Definition;

namespace Statecharts.NET
{
    public interface IBaseServiceDefinition
    {
        string Id { get; }
        UnguardedEventTransitionDefinition OnErrorTransition { get; }
    }

    public interface IServiceDefinition : IBaseServiceDefinition
    {
        Func<CancellationToken, Task> Task { get; }
        UnguardedEventTransitionDefinition OnSuccessDefinition { get; }
    }

    public interface IServiceDefinition<T> : IBaseServiceDefinition
    {
        Func<CancellationToken, Task<T>> Task { get; }
        UnguardedEventTransitionDefinition OnSuccessDefinition { get; } // TODO: add T
    }
}
