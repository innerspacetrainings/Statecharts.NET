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

        internal RelativeTarget(string statenodeName) => StatenodeName = statenodeName;
    }
    public class SiblingTarget : RelativeTarget 
    {
        public SiblingTarget(string statenodeName) : base(statenodeName) { }
    }
    public class ChildTarget : RelativeTarget 
    {
        public ChildTarget(string statenodeName) : base(statenodeName) { }
    }
    public class SelfTarget : Target { }
    public class UniquelyIdentifiedTarget : Target
    {
        public string Id { get; }
        public UniquelyIdentifiedTarget(string id) => Id = id;
    }
}