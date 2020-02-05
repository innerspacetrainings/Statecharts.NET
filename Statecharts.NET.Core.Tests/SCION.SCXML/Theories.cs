using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Xunit;

namespace Statecharts.NET.Tests.SCION.SCXML
{
    public class Theories
    {
        [Theory]
        [MemberData(nameof(GetTestSuite), "assign")]
        public void Assign(Test test) => TestStatechart(test);
        
        [Theory]
        [MemberData(nameof(GetTestSuite), "basic")]
        public void Basic(Test test) => TestStatechart(test);

        [Theory]
        [MemberData(nameof(GetTestSuite), "data")]
        public void Data(Test test) => TestStatechart(test);

        [Theory]
        [MemberData(nameof(GetTestSuite), "default-initial-state")]
        public void DefaultInitialState(Test test) => TestStatechart(test);

        [Theory]
        [MemberData(nameof(GetTestSuite), "documentOrder")]
        public void DocumentOrder(Test test) => TestStatechart(test);

        [Theory]
        [MemberData(nameof(GetTestSuite), "targetless-transition")]
        public void TargetlessTransition(Test test) => TestStatechart(test);

        private static void TestStatechart(Test test)
        {
            var scxmlDefinition = File.ReadAllText(test.Path);
            var definition = SCXMLEcmascript.TestXML(scxmlDefinition);
            var parsed = definition.Parse();

            Assert.IsType<ExecutableStatechart<EcmaScriptContext>>(parsed);

            var service = (parsed as ExecutableStatechart<EcmaScriptContext>).Interpret();

            Assert.Equal(service.Start().Ids(), test.Script.InitialConfiguration);

            foreach (var step in test.Script.Steps)
                Assert.Equal(service.Send(new Model.NamedEvent(step.Event.Name)).Ids(), step.NextConfiguration);
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
                        JsonConvert.DeserializeObject<TestScript>(File.ReadAllText($"{test.path}.json"))));

            var theoryData = new TheoryData<Test>();
            foreach(var test in testConfigurations)
                theoryData.Add(test);

            return theoryData;
        }
    }
}
