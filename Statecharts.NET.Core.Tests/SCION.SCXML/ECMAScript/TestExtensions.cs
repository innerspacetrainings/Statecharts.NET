using System.Collections.Generic;
using System.Linq;
using Statecharts.NET.Model;
using Statecharts.NET.Utilities;

namespace Statecharts.NET.Tests.SCION.SCXML.ECMAScript
{
    internal static class Extensions
    {
        internal static IList<string> Ids(this State<ECMAScriptContext> state)
        {
            IEnumerable<string> Recurse(StateConfiguration.Entry entry, IEnumerable<string> leaves) =>
                entry.Children.Count == 0
                    ? leaves.Concat(entry.StatenodeName.Yield())
                    : entry.Children.SelectMany(child => Recurse(child, leaves));
            return Recurse(state.StateConfiguration.Root, Enumerable.Empty<string>()).ToList();
        }
        ////=> state.StateConfiguration.NonRootIds.Reverse().Aggregate(Enumerable.Empty<string>(), (leaves, current) =>
        ////{
        ////    var leavesList = leaves.ToList();
        ////    return leavesList.Any(leave => leave.StartsWith(current))
        ////        ? leavesList
        ////        : leavesList.Append(current);
        ////}).Select(id => id.Split('.').LastOrDefault()).ToList();


        ////internal static IList<string> Ids(this State<ECMAScriptContext> state)
        ////{
        ////    var names = new List<string>();
        ////    void Recurse(StateConfiguration.Entry entry)
        ////    {
        ////        if (entry.Children.Count == 0)
        ////            names.Add(entry.StatenodeName);
        ////        else
        ////            foreach (var child in entry.Children)
        ////                Recurse(child);
        ////    }
        ////    Recurse(state.StateConfiguration.Root);
        ////    return names;
        ////}
    }
}
