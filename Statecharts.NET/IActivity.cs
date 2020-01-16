using System;
using System.Collections.Generic;
using System.Text;

namespace Statecharts.NET
{
    public interface IActivity
    {
        System.Action Start { get; set; }
        System.Action Stop { get; set; }
    }

    public class Activity : IActivity
    {
        public System.Action Start { get; set; }
        public System.Action Stop { get; set; }
    }
}
