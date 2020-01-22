using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Definition;
using Statecharts.NET.Model;
using Statecharts.NET.XState;
using static Statecharts.NET.XState.JPropertyConstructorFunctions;

namespace Statecharts.NET.Demo
{
    internal class FetchContext : IEquatable<FetchContext>, IXStateSerializable
    {
        public int Retries { get; set; }

        public bool Equals(FetchContext other) => other != null && Retries == other.Retries;

        ObjectValue IXStateSerializable.AsJSObject()
            => ObjectValue(("retries", Retries));

        public override string ToString()
            => $"FetchContext: (Retries = {Retries})";
    }

    internal static class Program
    {
        private static readonly StatechartDefinition<FetchContext> FetchDefinition = null;
            /*new StatechartDefinition<FetchContext>()
            {
                InitialContext = new FetchContext() { Retries = 0 },
                StateNodeDefinition = new CompoundStateNodeDefinition<FetchContext>()
                {
                    Name = "fetch",
                    InitialTransition = new InitialTransitionDefinition { Target = new ChildTargetDefinition() { Key = new NamedStateNodeKey("idle") } },
                    States = new List<BaseStateNodeDefinition<FetchContext>> {
                        new OrthogonalStateNodeDefinition<FetchContext>()
                        {
                            Name = "idle",
                            States = new List<BaseStateNodeDefinition<FetchContext>>()
                            {
                                new AtomicStateNodeDefinition<FetchContext>()
                                {
                                    Name = "really",
                                    Events = new List<BaseEventDefinition>
                                    {
                                        new EventDefinition<Event>()
                                        {
                                            Event = new Event("YES"),
                                            Transitions = new List<EventTransitionDefinition>()
                                            {
                                                new UnguardedEventTransitionDefinition()
                                                {
                                                    Targets = new List<BaseTargetDefinition>() { new AbsoluteTargetDefinition() { Id = new StateNodeId(new StateNodeKey[]{ new RootStateNodeKey("fetch"), new NamedStateNodeKey("loading") })} }
                                                }
                                            }
                                        },
                                        new EventDefinition<Event>()
                                        {
                                            Event = new Event("NO"),
                                            Transitions = new List<EventTransitionDefinition>()
                                            {
                                                new UnguardedEventTransitionDefinition()
                                                {
                                                    Targets = new List<BaseTargetDefinition>() { new SiblingTargetDefinition() { Key =  new NamedStateNodeKey("nana") } }
                                                }
                                            }
                                        }
                                    }
                                },
                                new AtomicStateNodeDefinition<FetchContext>()
                                {
                                    Name = "nana",
                                    Events = new List<BaseEventDefinition>
                                    {
                                        new EventDefinition<Event>()
                                        {
                                            Event = new Event("SERIOUSLY"),
                                            Transitions = new List<EventTransitionDefinition>()
                                            {
                                                new UnguardedEventTransitionDefinition()
                                                {
                                                    Targets = new List<BaseTargetDefinition>() { new AbsoluteTargetDefinition() { Id = new StateNodeId(new StateNodeKey[] { new RootStateNodeKey("fetch"), new NamedStateNodeKey("failure") }) } }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            Events = new List<BaseEventDefinition>
                            {
                                new EventDefinition<Event>()
                                {
                                    Event = new Event("FETCH"),
                                    Transitions = new List<EventTransitionDefinition>()
                                    {
                                        new UnguardedEventTransitionDefinition()
                                        {
                                            Targets = new List<BaseTargetDefinition>() { new SiblingTargetDefinition() { Key = new NamedStateNodeKey("loading") } }
                                        }
                                    }
                                }
                            }
                        },
                        new AtomicStateNodeDefinition<FetchContext>()
                        {
                            Name = "loading",
                            Events = new List<BaseEventDefinition>
                            {
                                new ImmediateEventDefinition()
                                {
                                    Transitions = new List<ImmediateTransitionDefinition>()
                                    {
                                        new GuardedImmediateTransitionDefinition()
                                        {
                                            Guard = new InlineGuard<FetchContext>() { Condition = (context, _) => context.Retries >= 3},
                                            Targets = new List<BaseTargetDefinition>() {new SiblingTargetDefinition() { Key = new NamedStateNodeKey("sheeeesh") } }
                                        }
                                    }
                                },
                                new EventDefinition<Event>()
                                {
                                    Event = new Event("RESOLVE"),
                                    Transitions = new List<EventTransitionDefinition>()
                                    {
                                        new UnguardedEventTransitionDefinition()
                                        {
                                            Targets = new List<BaseTargetDefinition>() { new SiblingTargetDefinition() { Key = new NamedStateNodeKey("success") } }
                                        }
                                    }
                                },
                                new EventDefinition<Event>()
                                {
                                    Event = new Event("REJECT"),
                                    Transitions = new List<EventTransitionDefinition>()
                                    {
                                        new UnguardedEventTransitionDefinition()
                                        {
                                            Targets = new List<BaseTargetDefinition>() { new SiblingTargetDefinition() { Key = new NamedStateNodeKey("failure") } }
                                        }
                                    }
                                }
                            },
                            EntryActions = new List<Action>()
                            {
                                new SideEffectAction<FetchContext>()
                                {
                                    Function = context => Console.WriteLine($"Entered loading state with context: {context}")
                                }
                            }
                        },
                        new FinalStateNodeDefinition<FetchContext>()
                        {
                            Name = "success"
                        },
                        new FinalStateNodeDefinition<FetchContext>()
                        {
                            Name = "sheeeesh"
                        },
                        new AtomicStateNodeDefinition<FetchContext>()
                        {
                            Name = "failure",
                            Events = new List<BaseEventDefinition>
                            {
                                new EventDefinition<Event>()
                                {
                                    Event = new Event("RETRY"),
                                    Transitions = new List<EventTransitionDefinition>()
                                    {
                                        new UnguardedEventTransitionDefinition()
                                        {
                                            Targets = new List<BaseTargetDefinition>() { new SiblingTargetDefinition() { Key = new NamedStateNodeKey("loading") } },
                                            Actions = new List<EventAction>()
                                            {
                                                new AssignEventAction<FetchContext, int>()
                                                {
                                                    Mutation = (context, _) => context.Retries++
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };*/

        private static void Main()
        {
            var definition = FetchDefinition;
            Console.WriteLine(definition.AsXStateDefinition("fetch"));

            var parsedStatechart = definition.Parse();
            Console.WriteLine($"Parsing the definition of the Statechart resulted in {parsedStatechart.GetType().Name}");

            switch (parsedStatechart)
            {
                case ExecutableStatechart<FetchContext> statechart:
                    var service = statechart.Interpret();
                    var state = service.Start();
                    Log(state);
                    while (true)
                    {
                        var eventType = Console.ReadLine();
                        state = service.Send(new Event(eventType?.ToUpper()));
                        Log(state);
                    }
                default:
                    Console.WriteLine("NOT EXECUTABLE");
                    break;
            }

        }

        private static void Log(State<FetchContext> state)
        {
            Console.WriteLine("StateConfig:");
            Console.WriteLine(string.Join(Environment.NewLine, state.StateConfiguration.StateNodeIds.Select(text => $"  {text}")));
        }
    }
}
