using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Statecharts.NET.Language;
using Statecharts.NET.XState;
using Xunit;
using static Statecharts.NET.Language.Keywords;


namespace Statecharts.NET.Tests.XState
{

    public class Tests
    {
        [Fact]
        public void Simple() => TestSerialization(
            Statechart
                .WithInitialContext(new DemoContext())
                .WithRootState("a".AsCompound().WithInitialState("a1").WithStates("a1")));

        private static void TestSerialization<TContext>(Definition.Statechart<TContext> statechart)
            where TContext : IEquatable<TContext>, IXStateSerializable
        {
            var schema = JSchema.Parse(File.ReadAllText(Path.GetRelativePath(Directory.GetCurrentDirectory(), $"XState/machine.schema.json")));
            var definition = JObject.Parse(statechart.AsXStateVisualizerV5Definition());

            Assert.True(definition.IsValid(schema));
        }
    }
}
