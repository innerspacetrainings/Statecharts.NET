using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Statecharts.NET
{
    public class StateNodeId : IEquatable<StateNodeId>
    {
        public IEnumerable<StateNodeKey> Path { get; }
        private string CachedAsString { get; }

        public StateNodeId(IEnumerable<StateNodeKey> path) // TODO: probably make this internal after creating the DSL
        {
            Path = path ?? throw new ArgumentNullException(nameof(path));
            CachedAsString = $"root:{string.Join(".", Path.Skip(1).Select(key => key.Map(_ => "root", named => named.StateName)))}";
        }

        internal StateNodeId(StateNodeId parentStateNodeId, StateNodeKey stateNodeKey) : this(parentStateNodeId.Path.Append(stateNodeKey)) { }
        internal StateNodeId(RootStateNodeKey rootStateNodeKey) : this(new []{rootStateNodeKey}) { }

        public override string ToString() => CachedAsString;

        public override bool Equals(object other)
            => other is StateNodeId id && Equals(id);
        
        public override int GetHashCode()
            => CachedAsString.GetHashCode();

        public bool Equals(StateNodeId other)
            => other != null && CachedAsString == other.CachedAsString;

        internal static StateNodeId Make(Interpreter.StateNode stateNode, NamedStateNodeKey key)
            => new StateNodeId(stateNode.Id, key);
    }

    public static class StateNodeKeyFunctions
    {
        public static TResult Map<TResult>(
            this StateNodeKey key,
            Func<RootStateNodeKey, TResult> fRoot,
            Func<NamedStateNodeKey, TResult> fNamed)
        {
            switch (key)
            {
                case RootStateNodeKey root:
                    return fRoot(root);
                case NamedStateNodeKey named:
                    return fNamed(named);
                default: throw new Exception("NON EXHAUSTIVE SWITCH");
            }
        }
    }

    public abstract class StateNodeKey
    {
    }

    public class RootStateNodeKey : StateNodeKey
    {
        public string RootstateName { get; }

        public RootStateNodeKey(string statechartName)
        {
            RootstateName = statechartName ?? throw new ArgumentNullException(nameof(statechartName));
        }

        public override bool Equals(object other) => other is RootStateNodeKey; // TODO: statechartName
        public override int GetHashCode() => 3189;
    }

    public class NamedStateNodeKey : StateNodeKey
    {
        public NamedStateNodeKey(string stateName) => StateName = stateName ?? throw new ArgumentNullException(nameof(stateName));

        public string StateName { get; }

        public override bool Equals(object other) => other is NamedStateNodeKey key && key.StateName == StateName;
        public override int GetHashCode() => StateName.GetHashCode();
    }
}
