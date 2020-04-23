using System;

namespace Statecharts.NET.Utilities.Time
{
    public static class TimeSpanExtensions
    {
        public static TimeSpan Seconds(this int seconds)
            => new TimeSpan(0, 0, 0, seconds);
        public static TimeSpan Seconds(this float seconds)
            => new TimeSpan(0, 0, 0, 0, (int)(seconds * 1000));
        public static TimeSpan Seconds(this double seconds)
            => new TimeSpan(0, 0, 0, 0, (int)(seconds * 1000));

        public static TimeSpan Milliseconds(this int milliseconds)
            => new TimeSpan(0, 0, 0, 0, milliseconds);
    }
}
