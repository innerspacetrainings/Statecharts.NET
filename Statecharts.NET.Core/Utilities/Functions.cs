using System;
using System.Collections.Generic;
using System.Text;

namespace Statecharts.NET.Utilities
{
    public static class Functions
    {
        public static T Identity<T>(T t) => t;
        public static void NoOp() { }
        public static void NoOp<T>(T t) { }
    }
}
