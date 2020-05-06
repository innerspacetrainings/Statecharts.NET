using Statecharts.NET.Utilities;

namespace Statecharts.NET.Model
{
    public class Target : OneOfBase<AbsoluteTarget, SiblingTarget, ChildTarget, SelfTarget, UniquelyIdentifiedTarget> { }

    public class AbsoluteTarget : Target
    {
        public StatenodeId Id { get; }

        public AbsoluteTarget(StatenodeId id) => Id = id;
    }
    public abstract class RelativeTarget : Target
    {
        public string StatenodeName { get; }
        public string[] ChildStatenodesNames { get; }

        internal RelativeTarget(string statenodeName, params string[] childStatenodesNames)
        {
            StatenodeName = statenodeName;
            ChildStatenodesNames = childStatenodesNames;
        }
    }
    public class SiblingTarget : RelativeTarget {
        public SiblingTarget(string statenodeName, params string[] childStatenodesNames) : base(statenodeName, childStatenodesNames) { }
    }
    public class ChildTarget : RelativeTarget {
        public ChildTarget(string statenodeName, params string[] childStatenodesNames) : base(statenodeName, childStatenodesNames) { }
    }
    public class SelfTarget : Target { }
    public class UniquelyIdentifiedTarget : Target
    {
        public string Id { get; }
        public UniquelyIdentifiedTarget(string id) => Id = id;
    }
}