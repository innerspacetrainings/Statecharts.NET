using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language;
using Statecharts.NET.Model;
using Xunit;


namespace Statecharts.NET.XState.Tests
{

    public class Tests
    {
        [Fact]
        public void Simple() => TestSerialization(
            Define.Statechart
                .WithInitialContext(new DemoContext())
                .WithRootState("a".AsCompound().WithInitialState("a1").WithStates("a1")));

        private static void TestSerialization<TContext>(StatechartDefinition<TContext> statechart)
            where TContext : IContext<TContext>, IXStateSerializable
        {
            var schema = JSchema.Parse(File.ReadAllText(Path.GetRelativePath(Directory.GetCurrentDirectory(), $"machine.schema.json")));
            var definition = JObject.Parse(statechart.AsXStateVisualizerV5Definition());

            Assert.True(definition.IsValid(schema));
        }
    }
}
