// ReSharper disable ClassNeverInstantiated.Global
using Newtonsoft.Json;

namespace Statecharts.NET.SCION.SCXML.Tests
{
    internal interface IRequest { }

    internal class LoadRequest : IRequest
    {
        [JsonProperty("Load")]
        public string FileName { get; set; }
    }

    internal class SendEventRequest : IRequest
    {
        public Event Event { get; set; }
        public int SessionToken { get; set; }
    }

    internal class InvalidRequest : IRequest { }

    internal class Event
    {
        public string Name { get; set; }
    }
}
