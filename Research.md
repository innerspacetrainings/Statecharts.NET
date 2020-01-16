## Implementations in other Languages
- https://github.com/sstuddard/Moe.StateMachine (C# 😍)
- https://github.com/AlexandreDecan/sismic
- https://www.boost.org/doc/libs/1_70_0/libs/statechart/doc/reference.html
- https://gitlab.com/scion-scxml/scion (https://github.com/jbeard4/SCION.NET)

## 2-Research
- Handling Events with Data
  - https://stackoverflow.com/questions/6968841/boost-statechart-pass-arguments-with-transition
  - http://boost.2283326.n4.nabble.com/statechart-Passing-parameters-from-event-to-State-Local-Storage-td2590845.html



merging json: https://stackoverflow.com/a/25621089
building DSLs in C#: https://www.codemag.com/Article/0902041/Building-Domain-Specific-Languages-in-C
statecharts comments: https://www.bcobb.net/hacker-school-read-along-statecharts/#fn1
C# async antipatterns: https://markheath.net/post/async-antipatterns
C# monads and links: https://devblogs.microsoft.com/pfxteam/tasks-monads-and-linq/


## Sismic Statechart Algorithm
```
/************** interpreter **************/
// execute
for(var macroStep = execute_once; macroStep != null; macroStep = execute_once)
return list of macroSteps

// execute_once
compute_steps
if (len(computed_steps > 0))
  execute_micro_steps
return macroStep

// compute_steps   # Compute and returns the next steps based on current configuration and event queues.
???   # statechart not initialized
select_event
select_transitions
???   # no transitions can be triggered
sort_transitions
???   # should the step consume an event
return create_steps

# originally integrated in execute_once.if
// execute_micro_steps
???   # consume event if it triggered a transition
for step in computed_steps:
    executed_steps += apply_step(step)
    executed_steps += stabilize()
return macroStep

// apply_step   # Apply given *MicroStep* on this statechart
state_for(entered_states)
state_for(exited_states)
for state in exited_states:
	execute exitActions
	currentConfig.remove(state)
if step.transition
	execute transitionActions
for state in entered_states:
	execute entryActions
	currentConfig.add(state)
sendEvents
return microStep

// stabilize
for(var microStep = create_stabilization_step; microStep != null; microStep = create_stabilization_step)
return list of microSteps

// create_stabilization_step
leaves = statechart.leaf_for(currentConfig)
states = statechart.state_for(leaves)
for state in states:
	is FinalState && ???   # self._statechart.parent_for(leaf.name) == self._statechart.root
	is OrthogonalState && statechart.children_for
	is Compoundstate && state.initial
return microStep

// select_event   # Return the next event to process. Internal events have priority over external ones.
return (internal, external).merge.first

// select_transitions
???   # select all eventless transitions and the one event matches, TODO: what has to be prioritized how
???   # evaluate guards and remove transitions that were rejected
return selected_transitions

// sort_transitions
return ???   # transitions should be ordered by document order by document order by default in this implementation

// create_steps
for transition in transitions:
	???   # handle internal transitions
	lca = (source, target).LCA
	last_before_lca = source.OneBeneath(lca)
	exited = [last_before_lca, last_before_lca.descendants].Where(isActive)
	entered = target.AncestorsUntil(lca).Reverse
	return microStep
return returned_steps

/************** statechart **************/
// leaf_for
return states.filter isLeave

// state_for
return states.lookup(key)

// children_for
return state_for.children

```


## Activities vs. Invoked Services
- https://github.com/davidkpiano/xstate/issues/418
- https://github.com/davidkpiano/xstate/issues/268#issuecomment-520243978




## What a Fuck Findings
- https://github.com/DiamondDynamo/StateChart/blob/master/StateChart/Program.cs
- https://github.com/jbeard4/SCION.NET (https://twitter.com/bemayr/status/1151477935687315456)
- https://github.com/amarax/ui-statechart-unity/blob/foundation/Assets/Editor/UIStatechartEditor.cs