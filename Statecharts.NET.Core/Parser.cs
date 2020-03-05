using System;
using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Interfaces;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;
using Action = Statecharts.NET.Model.Action;

namespace Statecharts.NET
{
    internal static class ConversionExtensions
    {
        internal static Actionblock
            Convert(this IEnumerable<OneOf<ActionDefinition, ContextActionDefinition>> actions) =>
            actions != null
                ? Actionblock.From(actions.Select(Action.From))
                : Actionblock.Empty();
    }

    public class Parser
    {
        private static Statenode ParseStatenode(
            StatenodeDefinition definition,
            Statenode parent,
            int documentIndex)
        {
            IEnumerable<Statenode> ParseSubstateNodes(
                IEnumerable<StatenodeDefinition> substateNodeDefinitions,
                Statenode recursedParent) =>
                substateNodeDefinitions.Select((substateDefinition, index) => ParseStatenode(substateDefinition, recursedParent, documentIndex + index));

            var name = definition.Name;
            var entryActions = definition.EntryActions.Convert();
            var exitActions = definition.ExitActions.Convert();

            Statenode CreateAtomicStatenode(AtomicStatenodeDefinition _)
                => new AtomicStatenode(parent, name, documentIndex, entryActions, exitActions);
            Statenode CreateFinalStatenode(FinalStatenodeDefinition _)
                => new FinalStatenode(parent, name, documentIndex, entryActions, exitActions);
            Statenode CreateCompoundStatenode(CompoundStatenodeDefinition compoundDefinition)
            {
                var statenode = new CompoundStatenode(parent, name, documentIndex, entryActions, exitActions);
                statenode.Statenodes = ParseSubstateNodes(compoundDefinition.Statenodes, statenode);
                return statenode;
            }
            Statenode CreateOrthogonalStatenode(OrthogonalStatenodeDefinition orthogonalDefinition)
            {
                var statenode = new OrthogonalStatenode(parent, name, documentIndex, entryActions, exitActions);
                statenode.Statenodes = ParseSubstateNodes(orthogonalDefinition.Statenodes, statenode);
                return statenode;
            }

            return definition.Match(
                CreateAtomicStatenode,
                CreateFinalStatenode,
                CreateCompoundStatenode, 
                CreateOrthogonalStatenode);
        }

        private static IDictionary<StatenodeId, Statenode> ToSortedDictionary(Statenode rootnode)
        {
            IEnumerable<Statenode> nodes = rootnode.CataFold<IEnumerable<Statenode>>(
                atomic => atomic.Yield(),
                final => final.Yield(),
                (compound, children) => compound.Append(children.SelectMany(Functions.Identity)),
                (orthogonal, children) => orthogonal.Append(children.SelectMany(Functions.Identity)));

            var sorted = nodes.OrderByDescending(statenode => statenode.Depth)
                .ThenBy(statenode => statenode.DocumentIndex);

            return sorted.ToDictionary(statenode => statenode.Id);
        }

        private static void ParseAndSetTransitions(IDictionary<StatenodeId, Statenode> statenodes)
        {
            foreach (var statenode in statenodes.Values)
            {
                statenode.Switch(
                    atomic => throw new NotImplementedException(), // atomic.Transitions = null,
                    final => { },
                    compound => throw new NotImplementedException(), // compound.Transitions = null,
                    orthogonal => throw new NotImplementedException()); // orthogonal.Transitions = null);
            }
        }

        // TODO: return actual ParsedStatechart based on results from parsing
        public static ParsedStatechart<TContext> Parse<TContext>(StatechartDefinition<TContext> definition)
            where TContext : IContext<TContext>
        {
            var rootStatenode = ParseStatenode(definition.RootStateNode, null, 0);
            var statenodes = ToSortedDictionary(rootStatenode);

            ParseAndSetTransitions(statenodes);

            return new ExecutableStatechart<TContext>(
                rootStatenode,
                definition.InitialContext.CopyDeep(),
                statenodes);
        }
    }
}
