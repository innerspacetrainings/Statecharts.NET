# Statecharts.NET

![Statecharts.NET.Core](https://github.com/innerspacetrainings/Statecharts.NET/workflows/Statecharts.NET.Core/badge.svg)

## Quick Example
```csharp
// Statechart Definition
var behaviour = Define.Statechart
    .WithInitialContext(new FetchContext { Retries = 0 })
    .WithRootState(
        "fetch"
            .AsCompound()
            .WithInitialState("idle")
            .WithStates(
                "idle".WithTransitions(
                    On("FETCH").TransitionTo.Sibling("loading")),
                "loading".WithTransitions(
                    On("RESOLVE").TransitionTo.Sibling("success"),
                    On("REJECT").TransitionTo.Sibling("failure")),
                "failure".WithTransitions(
					On("RETRY").TransitionTo.Sibling("loading").WithActions<FetchContext>(Assign<FetchContext>(context => context.Retries++))),
                "success".AsFinal()));

// Usage
var parsedStatechart = Parser.Parse(Behaviour);
var statechart = Interpreter.Interpret(parsedStatechart);

statechart.RunAsync();
statechart.Send(new NamedEvent("FETCH"));
/* ... */

// FetchContext
internal class FetchContext : IContext<FetchContext>, IXStateSerializable
{
    public int Retries { get; set; }

    public bool Equals(FetchContext other) => other != null && Retries == other.Retries;
    ObjectValue IXStateSerializable.AsJSObject() => ObjectValue(("retries", Retries));
    public FetchContext CopyDeep() => new FetchContext { Retries = Retries };
    public override string ToString() => $"FetchContext: (Retries = {Retries})";
}
```

## Features

### The DSL (Statecharts.NET.Language)

One of the main ideas of Statecharts.NET is to separate the Statechart Interpreter and the way you describe Statecharts.
This is one of the mightiest features of the Statecharts Concept.
*Statecharts.NET.Core* includes all the functionality for creating Statecharts and executing them.
*Statecharts.NET.Language* provides a reference implementation of how the core functionality can be wrapped to create a nicely looking syntax for creating Statecharts.
It is intended to be as functional/composable as possible while preserving the main idea of explicity/clarity.

> :info: **Make sure to add `using static Statecharts.NET.Language.Keywords;` to your usings!**

#### Your new best friend: `Define.<...>`

The API is built in a way that enables it to be easily explored via *IntelliSense/Autocomplete*.
If you want to define some [Statechart-Element](https://statecharts.github.io/glossary/) for reusability, just type "Define" followed by a "." to enable auto completion.
After that you will be guided most of the available features.

#### Element Definitions

```csharp
// Events
var Start = Define.Event("Start");
var IncrementBy = Define.EventWithData<int>("IncrementBy"); // can be sent to the Statechart via .Send(IncrementBy(...))

// Actions
var SideEffect1 = Define.Action.SideEffect(() => Console.WriteLine("I'm a Side Effect"));
var SideEffect2 = Define.Action.SideEffectWithContext<FetchContext>(context => Console.WriteLine($"I have access to the context {context.Retries}"));
var SideEffect3 = Define.Action.SideEffectWithContextAndData<FetchContext, int>((context, amount) => Console.WriteLine($"I have access to the context {context.Retries} and some data {amount}"));
var Assign1 = Define.Action.Assign<FetchContext>(context => context.Retries = 0);
var Assign2 = Define.Action.AssignWithData<FetchContext, int>((context, amount) => context.Retries += amount);

// Services
var TaskService = Define.Service.Task(token => Task.Delay(TimeSpan.FromSeconds(3), token));
var ActivityService = Define.Service.Activity(() => Console.WriteLine("started"), () => Console.WriteLine("stopped"));
```

#### Statechart Definition

> :info: The basic definition of a Statenode is simply a string. The Builder Methods are extension methods for string to reduce character cound and improve readability. Ask your autocompletion with **`.With<...>`** and **`.As<...>`** for help!

```csharp
// Root Statenode
var CompoundRoot = Define.Statechart
    .WithInitialContext(new NoContext())
    .WithRootState(
        "example"
            .AsCompound()
            .WithInitialState("first")
            .WithStates("first", "second"));
var OrthogonalRoot = Define.Statechart
    .WithInitialContext(new NoContext())
    .WithRootState(
        "example"
            .AsOrthogonal()
            .WithStates("a", "b"));

// Transitions
var TransitionsExample = "example".WithTransitions(
    On("EventName").TransitionTo.Self,
    On(Start).TransitionTo.Self,
    On(IncrementBy).TransitionTo.Self,
                
    On("dummy").If<FetchContext>(context => context.Retries > 10).TransitionTo.Self,
    On(IncrementBy).If<FetchContext>((_, amount) => amount > 5).TransitionTo.Self,
                
    Ignore("EventName"),
    Ignore(Start),
    Ignore(IncrementBy),
                
    Immediately.TransitionTo.Self,
                
    After(3.Seconds()).TransitionTo.Self,
                
    On("dummy").TransitionTo.Self,
    On("dummy").TransitionTo.Child("child", "even", "deep", "children"),
    On("dummy").TransitionTo.Sibling("sibling", "even", "children", "of", "siblings"),
    On("dummy").TransitionTo.Target(Sibling("sibling")),
    On("dummy").TransitionTo.Target(Child("child")),
    On("dummy").TransitionTo.Absolute("rootstatenode", "children", "deeper", "..."),
    On("dummy").TransitionTo.Multiple(Child("paralle", "child1"), Child("parallel", "child2")),
                
    On("dummy").TransitionTo.Self.WithActions(SideEffect1, Log("and another one")));

// Actions
var ActionsExample = "example".WithEntryActions<FetchContext>(
    Run(() => Console.WriteLine("some arbitrary action")),
    Run<FetchContext>(context => Console.WriteLine($"some arbitrary action with {context}")),
    Log("logging a label"),
    Log<FetchContext>(context => $"logging some context {context}"),
    Assign<FetchContext>(context => context.Retries = 0));

// Statenodes OnDone
var OnCompoundDoneExample = "example"
    .AsCompound()
    .WithInitialState("first")
    .WithStates("first".AsFinal())
    .OnDone.TransitionTo.Sibling("sibling");
var OnOrthogonalDoneExample = "example"
    .AsOrthogonal()
    .WithStates("first".AsFinal(), "second".AsFinal())
    .OnDone.TransitionTo.Sibling("sibling");

// Services OnSuccess (OnError is currently missing :/)
var TaskServiceExample = "example"
    .WithInvocations(TaskService.OnSuccess.TransitionTo.Sibling("sibling"));
```


### Reusing Elements in Statecharts

The DSL is built around a **type-safe** (as much as possible in C#) and **ordered** (built with git in mind) builder pattern.
But the builder methods are only used for a human-friendly, readabilty-optimized syntax, actually stitching together the Elements is done via parameters.
Most of the builder methods' function signatures look like `(T, params T)` or `(OneOf<T1, T2>, params OneOf<T1, T2>[])`.
The idea behind this is, that it doesn't make sense to call a builder method without any argument, but you can't specify this in C# using the `params` keyword, so a required first parameter is utilized in Statecharts.NET.
Please don't get confused by the usage of `OneOf` in the method signatures, currently this "hack" is used to model [tagged unions](https://en.wikipedia.org/wiki/Tagged_union) in C#.

So to **reuse elements in Statecharts.NET**, just create a function, a method or a property that returns the according element (it's just an object that is handled by Statecharts.NET at runtime) and then include it in the builder pattern.
It's as simple as that 🤓.
Just make sure to use the appropriate return type, then you can also utilize all the existing builder methods.

This is a simple, but also very dumb example:
```csharp
private static StatenodeDefinition Numbered(int number, Target next) =>
    $"Statenode{number}"
        .WithTransitions(Immediately.TransitionTo.Target(next));

// usage: .WithStates(..., Numbered(2, Sibling("another")))
```

### Executing a Statechart

```csharp
var parsedStatechart = Parser.Parse(Behaviour);
var statechart = Interpreter.Interpret(parsedStatechart, /* optionally takes InterpreterOptions where logging and timing behaviour can be defined */);

statechart.RunAsync(); // returns a Task that can be awaited
statechart.Send(new NamedEvent("FETCH"));

statechart.NextEvents; // returns the next possible events
statecharts.OnMacroStep += macrostep => { /* ... */ }; // provides introspection for the statechart execution behaviour
```

## Missing Features
- [History States](https://www.w3.org/TR/scxml/#history)
- Correctly Resolving Parallel States (see: https://www.w3.org/TR/scxml/#algorithm)
- Internal/External Transitions
- Pass Context to Services
- Statecharts without TContext
- OnError on TaskService/ActivityService

## Roadmap
- [x] Model the Statechart Types
- [x] Build basic xstate Serialization
- [x] Create an Interpreter
- [x] Set up [scion-scxml/test-framework](https://gitlab.com/scion-scxml/test-framework)
- [x] Set up easier Testing using xUnit (using Theories)
- [x] Clean up the File/Project Hierarchy
- [x] Add xstate-like "Invoked Services"
- [x] Create the DSL
- [x] Build Unity-Integration
- [x] Fix the xstate Serialization
- [x] Finish SCXML EcmaScript Parser
- [x] Fix the Algorithm
- ...
- [ ] Introduce the "Hole"-Concept into StatechartDefinition
- [ ] Build a wasm-based Visualization Tool
- ...
- ...
- [ ] Separate SCXML parsing from `datamodel`

## TODOs

- [ ] `<service>.Send(...)` should not be callable when `<service>.Start(...)` wasn't called previously
- [ ] Stricter Types for TargetDefinition and StateNode (child + sibling only on StateNodes with children or siblings)
- [ ] think about access modifiers
- [ ] document public things
- Missing Features
	- [ ] Deep Initial IDs (https://github.com/davidkpiano/xstate/issues/675)
	- [ ] In State Guards
	- [ ] Delayed Events
	- [ ] ObservableService
	- [ ] NestedStatechartService
	- [ ] DoneData
- Tooling
	- [ ] Add missing properties to xstate Serialization, and fix it (e.g. same event twice)

- Blog About
	- Testing using SCION.Tests & JInt
	- XElement to T Mapper
	- CataFold
	- params min 1 parameter
	- params OneOf
	- OneOf Treeshaking
	- Builder without .Build()
	- Builder with Order of With Functions (+ git advantages)
	- Builder with skippable With Functions