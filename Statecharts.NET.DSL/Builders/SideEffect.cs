using System;
using System.Collections.Generic;
using System.Text;

namespace Statecharts.NET.Language.SideEffect
{
    public class Builder
    {
        public Model.SideEffectContextAction Define(System.Action effect) =>
            new Model.SideEffectContextAction(_ => effect());
        public Model.SideEffectContextAction Define<T>(System.Action<T> effect) =>
            new Model.SideEffectContextAction(context => effect((T)context));
    }
}
