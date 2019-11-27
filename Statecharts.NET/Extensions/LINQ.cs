using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Statecharts.NET.Extensions
{
    static class LINQ
    {
        public static (IEnumerable<T>, IEnumerable<T>) Segment<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            var matched = new List<T>();
            var unmatched = new List<T>();

            foreach (var item in source)
                if (predicate(item))
                    matched.Add(item);
                else
                    unmatched.Add(item);

            return (matched, unmatched);
        }

        public static IEnumerable<T> Append<T>(
            this IEnumerable<T> source, params T[] tail)
            => source.Concat(tail);

        public static IEnumerable<T> Append<T>(
            this T source, params T[] tail)
            => new []{ source }.Concat(tail);

        public static IEnumerable<T> Append<T>(
            this T source, IEnumerable<T> tail)
            => new[] { source }.Concat(tail);

        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
