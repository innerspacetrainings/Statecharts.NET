using System.Collections.Generic;
using Newtonsoft.Json;
using Statecharts.NET.Utilities;
using Xunit.Abstractions;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript
{
    public class Test : IXunitSerializable
    {
        private string testScriptContent;

        public Test() { }
        public Test(string name, string path, string testScript, Option<string> skipReason) : this()
        {
            Name = name;
            Path = path;
            testScriptContent = testScript;
            SkipReason = skipReason;
        }

        public override string ToString() => Name;

        internal string Name { get; private set; }
        internal string Path { get; private set; }
        internal Option<string> SkipReason { get; private set; }
        public TestScript Script => JsonConvert.DeserializeObject<TestScript>(testScriptContent);

        public void Deserialize(IXunitSerializationInfo info)
        {
            Name = info.GetValue<string>("name");
            Path = info.GetValue<string>("path");
            testScriptContent = info.GetValue<string>("testScriptContent");
            SkipReason = info.GetValue<string>("skipReason").ToOption();
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("name", Name, typeof(string));
            info.AddValue("path", Path, typeof(string));
            info.AddValue("testScriptContent", testScriptContent, typeof(string));
            info.AddValue("skipReason", SkipReason.ValueOr((string) null), typeof(string));
        }
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
