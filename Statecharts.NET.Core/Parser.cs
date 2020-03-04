using System;
using System.Collections.Generic;
using System.Text;
using Statecharts.NET.Model;

namespace Statecharts.NET
{
    public class Parser
    {
        private static Statenode ParseStatenode(Statenode parent, StatenodeDefinition definition)
        {
            definition.Match(
                atomic => new AtomicStatenode(),
                final => new FinalStatenode(),
                compound => new CompoundStatenode(), 
                orthogonal => new OrthogonalStatenode());
            new CompoundStatenode(Transform(definition.InitialTransition))
        }

        public _ Parse(_)
    }
}
