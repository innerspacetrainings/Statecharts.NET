using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Statecharts.NET.Definition;

namespace Statecharts.NET.Language.Service
{
    public delegate Task ServiceLogic(CancellationToken cancellationToken);
    public delegate Task<T> ServiceLogic<T>(CancellationToken cancellationToken);

    internal class DefinitionData
    {
        public ServiceLogic Task { get; }
        public string Id { get; set; }
        public UnguardedEventTransitionDefinition OnErrorTransition { get; set; }
        public UnguardedEventTransitionDefinition OnSuccessDefinition { get; set; }

        public DefinitionData(ServiceLogic task) => Task = task ?? throw new ArgumentNullException(nameof(task));
    }

    public class WithLogic : WithId
    {
        public WithLogic(ServiceLogic task) : base(task) { }

        public WithId WithId(string id)
        {
            DefinitionData.Id = id;
            return this;
        }
    }
    public class WithId : WithOnSuccessHandler
    {
        internal WithId(ServiceLogic task) : base(task) { }

        public WithOnSuccessHandler OnSuccess(UnguardedEventTransitionDefinition transitionDefinition)
        {
            DefinitionData.OnSuccessDefinition = transitionDefinition;
            return this;
        }
    }
    public class WithOnSuccessHandler : WithOnErrorHandler
    {
        internal WithOnSuccessHandler(ServiceLogic task) : base(task) { }

        public WithOnErrorHandler OnError(UnguardedEventTransitionDefinition transitionDefinition)
        {
            DefinitionData.OnErrorTransition = transitionDefinition;
            return this;
        }
    }
    public class WithOnErrorHandler : IServiceDefinition
    {
        private protected DefinitionData DefinitionData { get; }

        internal WithOnErrorHandler(ServiceLogic logic) => DefinitionData = new DefinitionData(logic);

        public Func<CancellationToken, Task> Task => async token => await DefinitionData.Task(token); // TODO: Types
        public string Id => DefinitionData.Id;
        public UnguardedEventTransitionDefinition OnErrorTransition => DefinitionData.OnErrorTransition;
        public UnguardedEventTransitionDefinition OnSuccessDefinition => DefinitionData.OnSuccessDefinition;
    }
}
