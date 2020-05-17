namespace Statecharts.NET.Interfaces
{
    public interface IContext<TImplementing>
    {
        TImplementing CopyDeep();
    }
}
