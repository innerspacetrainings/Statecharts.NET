using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Language;
using Statecharts.NET.XState;
using static Statecharts.NET.Language.Keywords;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Statecharts.NET.Demo.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            var statechartDefinition = Statechart
                .WithInitialContext(new DemoContext())
                .WithRootState(
                    "door"
                        .AsCompound()
                        .WithInitialState("open")
                        .WithStates(
                            "open".WithTransitions(On("close").TransitionTo.Sibling("closed")),
                            "closed".WithTransitions(On("open").TransitionTo.Sibling("open"))));

            var statements = new[]
            {
                $@"ace.edit('brace-editor').setValue(""{statechartDefinition.AsXStateVisualizerV4Definition()}"")", // set editor content
                "document.querySelector('#brace-editor + div > button').click()", // update statechart
                "document.querySelector('#root > main > *:last-child').click()", // hide editor
                "document.querySelector('#root > main > header').remove()", // remove left header
                "document.querySelector('#root > main > div:nth-child(1)').remove()", // remove login button
                "document.querySelector('#root > main').setAttribute('style', 'grid-template-rows: 0 auto;')" // remove spacing left over from header
            };

            XStateDiagram.Opacity = 0;
            XStateDiagram.Source = new Uri(@"https://xstate.js.org/viz/");
            XStateDiagram.NavigationCompleted += async (_, __) =>
            {
                await XStateDiagram.InvokeScriptAsync("eval", new[] {string.Join(";", statements)});
                await Task.Delay(600);
                XStateDiagram.Opacity = 1;
            };
        }
    }

    public class DemoContext : IContext<DemoContext>, IXStateSerializable
    {
        public bool Equals(DemoContext other) => true;

        public DemoContext CopyDeep() => new DemoContext();
        public ObjectValue AsJSObject() => new ObjectValue(Enumerable.Empty<JSProperty>());
    }
}
