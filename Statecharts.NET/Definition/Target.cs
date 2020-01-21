using Statecharts.NET.Utilities;

namespace Statecharts.NET.Definition
{
    public class Target : OneOfBase<AbsoluteTarget, SiblingTarget, ChildTarget> { }

    public class AbsoluteTarget : Target
    {
        public StateNodeId Id { get; }

        public AbsoluteTarget(StateNodeId id) => Id = id;
    }

    public abstract class RelativeTarget : Target
    {
        public NamedStateNodeKey Key { get; }

        internal RelativeTarget(string stateNodeName) => Key = new NamedStateNodeKey(stateNodeName);
    }

    public class SiblingTarget : RelativeTarget {
        public SiblingTarget(string stateNodeName) : base(stateNodeName) { }
    }

    public class ChildTarget : RelativeTarget {
        public ChildTarget(string stateNodeName) : base(stateNodeName) { }
    }
}
