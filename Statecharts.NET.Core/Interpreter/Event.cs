using System;

namespace Statecharts.NET.Interpreter
{
    public class ServiceSuccessEvent
    {
        public ServiceSuccessEvent(string serviceId, object result)
        {
            ServiceId = serviceId;
            Result = result;
        }

        public string ServiceId { get; }
        public object Result { get; }
    }

    public class ServiceErrorEvent
    {
        public ServiceErrorEvent(string serviceId, Exception exception)
        {
            ServiceId = serviceId;
            Exception = exception;
        }

        public string ServiceId { get; }
        public Exception Exception { get; }
    }

    public class ExecutionErrorEvent
    {
        public ExecutionErrorEvent(Exception exception) => Exception = exception;
        public Exception Exception { get; }
    }
}
