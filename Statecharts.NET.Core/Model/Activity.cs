namespace Statecharts.NET.Model
{
    public class Activity
    {
        public System.Action Start { get; set; }
        public System.Action Stop { get; set; }

        public Activity(System.Action start, System.Action stop)
        {
            Start = start;
            Stop = stop;
        }
    }
}
