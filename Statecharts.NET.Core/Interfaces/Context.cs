using System;

namespace Statecharts.NET.Interfaces
{
    public interface IContext<TImplementing> : IEquatable<TImplementing>
    {
        TImplementing CopyDeep();
    }
}
