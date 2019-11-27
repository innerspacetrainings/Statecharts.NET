# Statecharts.NET

## Roadmap
- [x] Model the Statechart Types
- [x] Build basic xstate Serialization
- [x] Create an Interpreter
- [x] Set up [scion-scxml/test-framework](https://gitlab.com/scion-scxml/test-framework)
- [x] Set up easier Testing using xUnit (using Theories)
- [ ] Clean up the File/Project Hierarchy
- [ ] Add xstate-like "Invoked Services"
- [ ] Create the DSL
- [ ] Build Unity-Integration
- ...
- [ ] Fix the xstate Serialization
- [ ] Finish SCXML EcmaScript Parser
- [ ] Fix the Algorithm
- ...
- [ ] Introduce the "Hole"-Concept into StatechartDefinition
- [ ] Build a wasm-based Visualization Tool
- ...
- ...
- [ ] Separate SCXML parsing from `datamodel`

## TODOs

- Code Quality
	- [ ] Eventless Guard (for Eventless Transitions)
	- [ ] `<service>.Send(...)` should not be callable when `<service>.Start(...)` wasn't called previously
	- [ ] Prettify `Execute()`
	- [ ] unify `.Map(...)` in `CreateSteps(...)`
	- [ ] Remodel `CreateSteps(...)` and `SelectTransitions(...)` (@event is null)
	- [ ] think of InitialTransition in `CreateSteps(...)` and `SelectTransitions(...)`
	- [ ] `<T>.Equals(...)` vs. `operator==`
	- [ ] Stricter Types for TargetDefinition and StateNode (child + sibling only on StateNodes with children or siblings)
	- [ ] Handle empty Transitions, Actions, ...
	- [ ] think about access modifiers
	- [ ] document public things
	- [ ] constructors instead of setters

- Algorithm
	- [ ] Events (raised, sent, internal, external, priority)
	- [ ] Final State Handling
	- [ ] Execution Blocks (https://github.com/davidkpiano/xstate/issues/603)
	- [ ] Actually execute the Actions
	- [ ] Return Events from Action Execution
	- [ ] fix `SelectTransitions(...)` (so that not all are taken, document order, SCXML compatible)
	- Missing Features
		- [ ] Deep Initial IDs (https://github.com/davidkpiano/xstate/issues/675)
		- [ ] In State Guards
		- [ ] Delayed Events

- Tooling
	- [ ] Add missing properties to xstate Serialization, and fix it (e.g. same event twice)
	- [ ] Improve SCXML Test-Server
	- [ ] create the DSL
	- [ ] separate SCXML Parsing & Testing

- Non-Important Bugs
	- [ ] CurrentConfiguration (unify, expose)
	- [ ] `RootStateNodeKey` find a way to preserve the Statechart's name
	- [ ] Get real Statechart Id
