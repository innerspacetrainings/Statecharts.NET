using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;

namespace Statecharts.NET.Definition
{
    public static class TransitionDefinitionFunctions
    {
        public static IEnumerable<Target> GetTargets(this TransitionDefinition transition) =>
            transition.Match(
                forbidden => Enumerable.Empty<Target>(),
                unguarded => unguarded.Targets,
                unguarded => unguarded.Targets,
                unguarded => unguarded.Targets,
                guarded => guarded.Targets,
                guarded => guarded.Targets,
                guarded => guarded.Targets);
    }
}
