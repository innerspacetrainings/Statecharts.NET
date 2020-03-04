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
        public abstract Task Task { get; }
        public abstract Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition>> OnSuccessDefinition { get; }
    }
    public abstract class TaskDataServiceDefinition : ServiceDefinition
    {
        public abstract Task<object> Task { get; }
        public abstract Option<OneOfUnion<TransitionDefinition, UnguardedTransitionDefinition, UnguardedContextTransitionDefinition, UnguardedContextDataTransitionDefinition>> OnSuccessDefinition { get; }
    }
    #endregion
    #region Parsed
    public class Service { }
    #endregion
}
