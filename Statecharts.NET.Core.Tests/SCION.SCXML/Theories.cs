using System;
using System.IO;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Tests.SCION.SCXML.ECMAScript;
using Statecharts.NET.Utilities;
using Xunit;
using Xunit.Sdk;

namespace Statecharts.NET.Tests.SCION.SCXML
{
    public class Theories
    {
        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "actionSend")]
        public void ActionSend(Test test) => TestStatechart(test);

        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "assign", new[] { "assign_obj_literal:Deep Initial States aren't yet supported..." })]
        public void Assign(Test test) => TestStatechart(test);

        [SkippableTheory(Skip = "Nothing working yet...")]
        [MemberData(nameof(GetTestSuite), "assign-current-small-step")]
        public void AssignCurrentSmallStep(Test test) => TestStatechart(test);

        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "atom3-basic-tests")]
        public void Atom3BasicTests(Test test) => TestStatechart(test);

        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "basic")]
        public void Basic(Test test) => TestStatechart(test);

        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "cond-js")]
        public void CondJs(Test test) => TestStatechart(test);

        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "default-initial-state")]
        public void DefaultInitialState(Test test) => TestStatechart(test);

        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "documentOrder")]
        public void DocumentOrder(Test test) => TestStatechart(test);

        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "hierarchy")]
        public void Hierarchy(Test test) => TestStatechart(test);

        [SkippableTheory]
        [MemberData(nameof(GetTestSuite), "hierarchy+documentOrder")]
        public void HierarchyAndDocumentOrder(Test test) => TestStatechart(test);

        private static void TestStatechart(Test test)
        {
            test.SkipReason.SwitchSome(reason => throw new SkipException(reason));

            var scxmlDefinition = File.ReadAllText(test.Path);
            var definition = SCXMLECMAScriptParser.ParseStatechart(scxmlDefinition);

            var parsed = Parser.Parse(definition);

            Assert.IsType<ExecutableStatechart<ECMAScriptContext>>(parsed);

            var statechart = parsed as ExecutableStatechart<ECMAScriptContext>;
            var initialState = Resolver.ResolveInitialState(statechart).Match(s => s, exception => null); // TODO: quick fix

            Assert.Equal(test.Script.InitialConfiguration, initialState.Ids());

            var state = initialState;
            foreach (var step in test.Script.Steps)
            {
                state = Resolver.ResolveNextState(statechart, state, new NamedEvent(step.Event.Name)).Match(s => s, exception => null); // TODO: quick fix
                Assert.Equal(step.NextConfiguration, state.Ids());
            }
        }

        public static TheoryData<Test> GetTestSuite(string @case) =>
            GetTestSuite(@case, new string[0]);
        public static TheoryData<Test> GetTestSuite(string @case, string[] excludedStrings)
        {
            var testsDirectory = Path.GetRelativePath(Directory.GetCurrentDirectory(), $"SCION.SCXML/tests/{@case}");
            var excluded = excludedStrings.Select(data =>
            {
                var splitted = data.Split(":", 2).ToArray();
                return (name: splitted[0], reason: splitted[1]);
            }).ToDictionary(e => e.name, e => e.reason);

            if (!Directory.Exists(testsDirectory)) throw new ArgumentException($"Could not find case: {@case} ({testsDirectory} does not exist)");

            var testConfigurations = Directory.GetFiles(testsDirectory, "*.scxml")
                .Select(Path.GetFileName)
                .Select(name => name.Replace(".scxml", string.Empty))
                .Select(name => (name, path: Path.Join(testsDirectory, name)))
                .Select(test =>
                    new Test(
                        test.name,
                        $"{test.path}.scxml",
                        File.ReadAllText($"{test.path}.json"),
                        excluded.ContainsKey(test.name) ? Option.From(excluded[test.name]) : Option.None<string>()));

            var theoryData = new TheoryData<Test>();
            foreach (var test in testConfigurations)
                theoryData.Add(test);

            return theoryData;
        }
    }
}
