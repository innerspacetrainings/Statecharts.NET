using System;
using System.IO;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Tests.SCION.SCXML.ECMAScript;
using Xunit;

namespace Statecharts.NET.Tests.SCION.SCXML
{
    public class Theories
    {
        [Theory]
        [MemberData(nameof(GetTestSuite), "basic")]
        public void Basic(Test test) => TestStatechart(test);

        [Theory]
        [MemberData(nameof(GetTestSuite), "default-initial-state")]
        public void DefaultInitialState(Test test) => TestStatechart(test);

        [Theory]
        [MemberData(nameof(GetTestSuite), "documentOrder")]
        public void DocumentOrder(Test test) => TestStatechart(test);

        private static void TestStatechart(Test test)
        {
            var scxmlDefinition = File.ReadAllText(test.Path);
            var definition = SCXMLECMAScriptParser.ParseStatechart(scxmlDefinition);

            var parsed = Parser.Parse(definition);

            Assert.IsType<ExecutableStatechart<ECMAScriptContext>>(parsed);

            var statechart = parsed as ExecutableStatechart<ECMAScriptContext>;
            var initialState = Resolver.ResolveInitialState(statechart);

            Assert.Equal(test.Script.InitialConfiguration, initialState.Ids());

            var state = initialState;
            foreach (var step in test.Script.Steps)
            {
                state = Resolver.ResolveNextState(statechart, state, new NamedEvent(step.Event.Name));
                Assert.Equal(step.NextConfiguration, state.Ids());
            }
        }
        public static TheoryData<Test> GetTestSuite(string _case)
        {
            var testsDirectory = Path.GetRelativePath(Directory.GetCurrentDirectory(), $"SCION.SCXML/tests/{_case}");

            if (!Directory.Exists(testsDirectory)) throw new ArgumentException($"Could not find case: {_case} ({testsDirectory} does not exist)");

            var testConfigurations = Directory.GetFiles(testsDirectory, "*.scxml")
                .Select(Path.GetFileName)
                .Select(name => name.Replace(".scxml", string.Empty))
                .Select(name => (name, path: Path.Join(testsDirectory, name)))
                .Select(test => 
                    new Test(
                        test.name,
                        $"{test.path}.scxml",
                        File.ReadAllText($"{test.path}.json")));

            var theoryData = new TheoryData<Test>();
            foreach(var test in testConfigurations)
                theoryData.Add(test);

            return theoryData;
        }
    }
}
