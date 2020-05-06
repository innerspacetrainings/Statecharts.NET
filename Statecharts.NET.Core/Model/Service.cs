using System.Threading;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    #region Definition
    public abstract class ServiceDefinition : OneOfBase<ActivityServiceDefinition, TaskServiceDefinition, TaskDataServiceDefinition>
    {
        public abstract Option<string> Id { get; }
        public abstract Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnErrorTransition { get; }
    }

    public abstract class ActivityServiceDefinition : ServiceDefinition
    {
        public abstract Activity Activity { get; }
    }

    public abstract class TaskServiceDefinition : ServiceDefinition
    {
        public abstract TaskDelegate Task { get; }
        public abstract Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition { get; }
    }
    public abstract class TaskDataServiceDefinition : ServiceDefinition
    {
        public abstract TaskDelegate<object> TaskDelegate { get; }
        public abstract Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition, UnguardedContextDataTransitionDefinition>> OnSuccessDefinition { get; }
    }
    #endregion
    #region Parsed
    public class Service
    {
        public string Id { get; }
        private readonly TaskDelegate<object> _taskDelegate;

        internal Service(string id, TaskDelegate<object> taskDelegate)
        {
            Id = id;
            _taskDelegate = taskDelegate;
        }

        public System.Threading.Tasks.Task<object> Invoke(CancellationToken cancellationToken) => _taskDelegate(cancellationToken);
    }
    #endregion
}
