# Statecharts.NET

![Statecharts.NET.Core](https://github.com/innerspacetrainings/Statecharts.NET/workflows/Statecharts.NET.Core/badge.svg)

## Example
```csharp
static readonly StatechartDefinition<FetchContext> Behaviour = Statechart
    .WithInitialContext(new FetchContext { Retries = 0 })
    .WithRootState(
        "demo"
            .WithEntryActions(Run(() => Console.WriteLine("NOW THIS WORKS AS WELL 🎉")))
            .AsCompound()
            .WithInitialState("1")
            .WithStates(
                "1".WithTransitions(
                    On("START").TransitionTo.Sibling("mc"),
                    On(IncrementBy).TransitionTo.Self.WithActions<FetchContext>(Assign<FetchContext, int>((context, amount) => context.Retries += amount)),
                    On(Increment).TransitionTo.Self.WithActions<FetchContext>(Assign<FetchContext>(context => context.Retries++))),
                "mc".WithTransitions(On("RETRY").TransitionTo.Child("initial"))
                    .AsCompound().WithInitialState("initial").WithStates(
                    "initial".WithTransitions(
                        On("START").TransitionTo.Sibling("selecting")),
                    "selecting".WithTransitions(
                        On("CORRECT").TransitionTo.Sibling("solved")),
                    "solved".AsFinal())
                    .OnDone.TransitionTo.Sibling("final"),
                "final".AsFinal()));

// Usage
var parse = Parser.Parse(Behaviour) as ExecutableStatechart<FetchContext>;
var statechart = Interpreter.Interpret(statechart);
statechart.Start();
```

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