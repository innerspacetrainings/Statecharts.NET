using System.Collections.Generic;
using Newtonsoft.Json;

namespace Statecharts.NET.Tests.SCION.SCXML
{

    public class Test
    {
        public Test(string name, string path, TestScript script)
        {
            Name = name;
            Path = path;
            Script = script;
        }

        public override string ToString() => Name;

        internal string Name { get; }
        internal string Path { get; }
        public TestScript Script { get; }
    }

    public class TestScript
    {
        public IEnumerable<string> InitialConfiguration { get; set; }

        [JsonProperty("events")]
        public IEnumerable<Step> Steps { get; set; }
    }

    public class Step
    {
        public Event Event { get; set; }
        public int? After { get; set; }
        public IEnumerable<string> NextConfiguration { get; set; }
    }

    public class Event
    {
        public string Name { get; set; }
    }
}
