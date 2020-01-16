using System;
using Statecharts.NET.Interpreter;

namespace Statecharts.NET.Definition
{
    public static class BaseTargetDefinitionFunctions
    {
        public static TResult Map<TResult>(
            this BaseTargetDefinition targetDefinition,
            Func<AbsoluteTargetDefinition, TResult> fAbsoluteTarget,
            Func<SiblingTargetDefinition, TResult> fSiblingTarget,
            Func<ChildTargetDefinition, TResult> fChildTarget)
        {
            switch (targetDefinition)
            {
                case AbsoluteTargetDefinition parent:
                    return fAbsoluteTarget(parent);
                case SiblingTargetDefinition sibling:
                    return fSiblingTarget(sibling);
                case ChildTargetDefinition child:
                    return fChildTarget(child);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }

        public static TResult Map<TResult>(
            this RelativeTargetDefinition relativeTargetDefinition,
            Func<SiblingTargetDefinition, TResult> fSiblingTarget,
            Func<ChildTargetDefinition, TResult> fChildTarget)
        {
            switch (relativeTargetDefinition)
            {
                case SiblingTargetDefinition sibling:
                    return fSiblingTarget(sibling);
                case ChildTargetDefinition child:
                    return fChildTarget(child);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    public class BaseTargetDefinition { }

    public class AbsoluteTargetDefinition : BaseTargetDefinition
    {
        public StateNodeId Id { get; }

        public AbsoluteTargetDefinition(StateNodeId id) => Id = id;
    }

    public abstract class RelativeTargetDefinition : BaseTargetDefinition
    {
        public NamedStateNodeKey Key { get; }

        internal RelativeTargetDefinition(string stateNodeName) => Key = new NamedStateNodeKey(stateNodeName);
    }

    public class SiblingTargetDefinition : RelativeTargetDefinition {
        public SiblingTargetDefinition(string stateNodeName) : base(stateNodeName) { }
    }

    public class ChildTargetDefinition : RelativeTargetDefinition {
        public ChildTargetDefinition(string stateNodeName) : base(stateNodeName) { }
    }
}
