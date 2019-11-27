using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Statecharts.NET.SCXML
{
    class ExitOrder : IComparer<IState>
    {
        public int Compare(IState x, IState y)
        {
            throw new NotImplementedException();
        }
    }

    class EntryOrder : IComparer<IState>
    {
        public int Compare(IState x, IState y)
        {
            throw new NotImplementedException();
        }
    }

    class DocumentOrder : IComparer<IState>
    {
        public int Compare(IState x, IState y)
        {
            throw new NotImplementedException();
        }
    }

    class TransitionDocumentOrder : IComparer<ITransition>
    {
        public int Compare(ITransition x, ITransition y)
        {
            throw new NotImplementedException();
        }
    }

    class Interpreter
    {
        private SortedSet<IState> configuration;
        private SortedSet<IState> statesToInvoke;
        private Queue<string> internalQueue;
        private BlockingCollection<string> externalQueue;
        private bool running;

        public void Interpret(IStatechart statechart)
        {
            configuration = new SortedSet<IState>();
            statesToInvoke = new SortedSet<IState>();
            internalQueue = new Queue<string>();
            externalQueue = new BlockingCollection<string>();
            running = true;
            EnterStates(new[] { statechart.InitialTransition });
            MainEventLoop();
        }

        private void MainEventLoop()
        {
            while (running)
            {
                SortedSet<ITransition> enabledTransitions = null;
                var macroStepDone = false;

                // Here we handle eventless transitions and transitions triggered by internal events until macrostep is complete
                while (running && !macroStepDone)
                {
                    enabledTransitions = SelectEventlessTransitions();
                    if (!enabledTransitions.Any())
                    {
                        if (!internalQueue.Any())
                        {
                            macroStepDone = true;
                        }
                        else
                        {
                            var internalEvent = internalQueue.Dequeue();
                            // TODO: datamodel["_event"] = internalEvent
                            enabledTransitions = SelectTransitions(internalEvent);
                        }
                    }
                    else
                    {
                        Microstep(enabledTransitions.ToList());
                    }
                }

                // either we're in a final state, and we break out of the loop
                if (!running) break;

                // Invoking may have raised internal error events and we iterate to handle them
                if (internalQueue.Any()) continue;

                // A blocking wait for an external event.  Alternatively, if we have been invoked
                // our parent session also might cancel us.  The mechanism for this is platform specific,
                // but here we assume it’s a special event we receive
                var externalEvent = externalQueue.Take();
                if (IsCancelEvent(externalEvent))
                {
                    running = false;
                    continue;
                }
                // TODO: datamodel["_event"] = externalEvent
                foreach(var state in configuration)
                {
                    // TODO: do something with invoke
                }
                enabledTransitions = SelectTransitions(externalEvent);
                if(enabledTransitions.Any())
                {
                    Microstep(enabledTransitions.ToList());
                }
            }
            ExitInterpreter();
        }

        private void ExitInterpreter()
        {
            var statesToExit = configuration.ToList().OrderBy(state => state, new ExitOrder());
            foreach(var state in statesToExit)
            {
                foreach (var action in state.ExitActions)
                    action();
                configuration.Remove(state);
                if(IsFinalState(state) && IsRootState(state))
                {
                    ReturnDoneEvent(state);
                }
            }
        }

        private void ReturnDoneEvent(IState state)
        {
            externalQueue.Add("done.invoke.THISMACHINE"); // TODO: id
        }

        private bool IsCancelEvent(object externalEvent)
        {
            return false; // TODO: what is happening here
        }

        private void Microstep(IEnumerable<ITransition> enabledTransitions)
        {
            ExitStates(enabledTransitions);
            ExecuteTransitionContent(enabledTransitions);
            EnterStates(enabledTransitions);
        }

        private void EnterStates(IEnumerable<ITransition> enabledTransitions)
        {
            var statesToEnter = new SortedSet<IState>();
            var statesForDefaultEntry = new SortedSet<IState>();
            // TODO: history
            // initialize the temporary table for default content in history states
            ComputeEntrySet(enabledTransitions, statesToEnter, statesForDefaultEntry);
            foreach(var s in statesToEnter.OrderBy(state => state, new EntryOrder())) {
                configuration.Add(s);
                statesToInvoke.Add(s);
                foreach (var action in s.EntryActions) // documentOrder
                    action();
                if (statesForDefaultEntry.Contains(s))
                    foreach (var action in s.InitialTransition.Actions)
                        action();
                // TODO: history
                if (IsFinalState(s))
                    if (IsRootState(s))
                        running = false;
                    else {
                        var parent = s.Parent;
                        var grandparent = parent.Parent;
                        internalQueue.Enqueue("done.state." + parent.Id); // TODO: donedata
                        if (IsParallelState(grandparent))
                            if (GetChildStates(grandparent).All(IsInFinalState))
                                internalQueue.Enqueue("done.state." + grandparent.Id);
                    }
            }
        }

        private bool IsInFinalState(IState s)
        {
            if (IsCompoundState(s))
                return GetChildStates(s).Any(state => IsFinalState(s) && configuration.Contains(state));
            else if (IsParallelState(s))
                return GetChildStates(s).All(IsInFinalState);
            else
                return false;
        }

        private void ComputeEntrySet(IEnumerable<ITransition> transitions, SortedSet<IState> statesToEnter, SortedSet<IState> statesForDefaultEntry)
        {
            foreach(var t in transitions)
            {
                foreach (var s in t.Target)
                    AddDescendantStatesToEnter(s, statesToEnter, statesForDefaultEntry);
                var ancestor = GetTransitionDomain(t);
                foreach (var s in GetEffectiveTargetStates(t))
                    AddAncestorStatesToEnter(s, ancestor, statesToEnter, statesForDefaultEntry);
            }
        }

        private void AddAncestorStatesToEnter(IState state, IState ancestor, SortedSet<IState> statesToEnter, SortedSet<IState> statesForDefaultEntry)
        {
            foreach(var anc in GetProperAncestors(state, ancestor))
            {
                statesToEnter.Add(anc);
                if (IsParallelState(anc))
                    foreach (var child in GetChildStates(anc))
                        if (!statesToEnter.Any(s => IsDescendant(s, child)))
                            AddDescendantStatesToEnter(child, statesToEnter, statesForDefaultEntry);
            }
        }

        private IEnumerable<IState> GetEffectiveTargetStates(ITransition transition)
        {
            var targets = new SortedSet<IState>();
            foreach(var s in transition.Target)
            {
                // TODO: history
                targets.Add(s);
            }
            return targets;
        }

        private void AddDescendantStatesToEnter(IState state, SortedSet<IState> statesToEnter, SortedSet<IState> statesForDefaultEntry)
        {
            // TODO: history
            statesToEnter.Add(state);
            if (IsCompoundState(state))
            {
                statesForDefaultEntry.Add(state);
                foreach (var s in state.InitialTransition.Target)
                    AddDescendantStatesToEnter(s, statesToEnter, statesForDefaultEntry);
                foreach (var s in state.InitialTransition.Target)
                    AddAncestorStatesToEnter(s, state, statesToEnter, statesForDefaultEntry);
            }
            else
                if (IsParallelState(state))
                foreach (var child in GetChildStates(state))
                    if (!statesToEnter.Any(s => IsDescendant(s, child)))
                        AddDescendantStatesToEnter(child, statesToEnter, statesForDefaultEntry);
        }

        private void ExecuteTransitionContent(IEnumerable<ITransition> enabledTransitions)
        {
            foreach (var t in enabledTransitions)
                foreach (var action in t.Actions)
                    action();
        }

        private void ExitStates(IEnumerable<ITransition> enabledTransitions)
        {
            var statesToExit = ComputeExitSet(enabledTransitions);
            foreach (var s in statesToExit)
                statesToInvoke.Remove(s);
            statesToExit = statesToExit.OrderBy(state => state, new ExitOrder());
            // TODO: handle history
            foreach (var s in statesToExit)
            {
                foreach (var action in s.ExitActions) // TODO: sort by document order
                    action();
                configuration.Remove(s);
            }
        }

        private SortedSet<ITransition> SelectTransitions(string internalEvent)
        {
            var enabledTransitions = new SortedSet<ITransition>();
            var atomicStates = configuration.Where(IsAtomicState).OrderBy(state => state, new DocumentOrder());
            foreach (var state in atomicStates)
            {
                foreach (var s in new[] { state }.Concat(GetProperAncestors(state, null)))
                {
                    foreach (var t in s.Transitions.OrderBy(transition => transition, new TransitionDocumentOrder()))
                    {
                        if (t.Event == internalEvent && ConditionMatch(t))
                        {
                            enabledTransitions.Add(t);
                            goto OuterLoopEnd;
                        }
                    }
                OuterLoopEnd:;
                }
            }
            enabledTransitions = RemoveConflictingTransitions(enabledTransitions);
            return enabledTransitions;
        }

        private SortedSet<ITransition> RemoveConflictingTransitions(SortedSet<ITransition> enabledTransitions)
        {
            var filteredTransitions = new SortedSet<ITransition>();
            //toList sorts the transitions in the order of the states that selected them
            foreach (var t1 in enabledTransitions) {
                var t1Preempted = false;
                var transitionsToRemove = new SortedSet<ITransition>();
                foreach (var t2 in filteredTransitions) {
                    if (ComputeExitSet(new[] { t1 }).Intersect(ComputeExitSet(new[] { t2 })).Any()) {
                        if (IsDescendant(t1.Source, t2.Source))
                            transitionsToRemove.Add(t2);
                        else
                        {
                            t1Preempted = true;
                            break;
                        }
                    }
                }
                if(!t1Preempted)
                {
                    foreach (var t3 in transitionsToRemove)
                        filteredTransitions.Remove(t3);
                    filteredTransitions.Add(t1);
                }
            }

            return filteredTransitions;
        }

        private IEnumerable<IState> ComputeExitSet(IEnumerable<ITransition> transitions)
        {
            var statesToExit = new SortedSet<IState>();
            foreach (var t in transitions) {
                if(t.Target != null)
                {
                    var domain = GetTransitionDomain(t);
                    foreach (var s in configuration)
                        if (IsDescendant(s, domain))
                            statesToExit.Add(s);
                    }
                }
            return statesToExit;
        }

        private IState GetTransitionDomain(ITransition t)
        {
            var tstates = GetEffectiveTargetStates(t);
            if (!tstates.Any())
                return null;
            else if (t.Type == "internal" && IsCompoundState(t.Source) && tstates.All(s => IsDescendant(s, t.Source)))
                return t.Source;
            else
                return findLCCA(new[] { t.Source }.Concat(tstates));
        }

        private IState findLCCA(IEnumerable<IState> states)
        {
            foreach (var anc in GetProperAncestors(states.FirstOrDefault(), null).Where(s => IsCompoundState(s) || IsRootState(s)))
                if (states.Skip(1).All(s => IsDescendant(s, anc)))
                    return anc;
            return null; // TODO: this should not be reached
        }

        private SortedSet<ITransition> SelectEventlessTransitions()
        {
            var enabledTransitions = new SortedSet<ITransition>();
            var atomicStates = configuration.Where(IsAtomicState).OrderBy(state => state, new DocumentOrder());
            foreach (var state in atomicStates)
            {
                foreach (var s in new[] { state }.Concat(GetProperAncestors(state, null)))
                {
                    foreach (var t in s.Transitions.OrderBy(transition => transition, new TransitionDocumentOrder()))
                    {
                        if (t.Event == null && ConditionMatch(t))
                        {
                            enabledTransitions.Add(t);
                            goto OuterLoopEnd;
                        }
                    }
                OuterLoopEnd:;
                }
            }
            enabledTransitions = RemoveConflictingTransitions(enabledTransitions);
            return enabledTransitions;
        }

        private IEnumerable<IState> GetProperAncestors(IState state, object p)
        {
            throw new NotImplementedException();
        }

        private bool IsDescendant(IState source1, IState source2)
        {
            throw new NotImplementedException();
        }

        private IEnumerable<IState> GetChildStates(IState state)
        {
            throw new NotImplementedException();
        }

        // ----------------------

        private bool ConditionMatch(ITransition t)
        {
            return t.Condition();
        }

        private bool IsRootState(IState state)
        {
            throw new NotImplementedException();
        }

        private bool IsFinalState(IState state)
        {
            throw new NotImplementedException();
        }

        private bool IsAtomicState(IState state)
        {
            throw new NotImplementedException();
        }

        private bool IsCompoundState(IState s)
        {
            throw new NotImplementedException();
        }

        private bool IsParallelState(IState state)
        {
            throw new NotImplementedException();
        }

    }
}
