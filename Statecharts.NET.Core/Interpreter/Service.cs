using System;
using System.Threading;
using System.Threading.Tasks;

namespace Statecharts.NET.Interpreter
{
    public class Service
    {
        public string Id { get; }
        private readonly Model.Task<object> _task;

        private Service(string id, Model.Task<object> task)
        {
            Id = id;
            _task = task;
        }

        public Task<object> Invoke(CancellationToken cancellationToken)
            => _task(cancellationToken);

        public static Service FromDefinition(Definition.Service serviceDefinition)
        {
            Service CreateServiceFromActivity(Definition.ActivityService service) =>
                new Service(service.Id, token =>
                {
                    token.Register(service.Activity.Stop);
                    service.Activity.Start(); // TODO: handle failure
                    return new TaskCompletionSource<object>().Task; // TODO: check if token and TaskCompletionSource have to be linked
                });

            return serviceDefinition.Match(
                CreateServiceFromActivity,
                task => new Service(serviceDefinition.Id, async cancellationToken =>
                {
                    await task.Task(cancellationToken);
                    return default;
                }),
                dataTask => new Service(serviceDefinition.Id, dataTask.Task));
        }
    }
}
