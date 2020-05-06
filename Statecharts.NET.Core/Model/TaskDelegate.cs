using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Statecharts.NET.Model
{
    public delegate System.Threading.Tasks.Task TaskDelegate(CancellationToken cancellationToken);
    public delegate System.Threading.Tasks.Task<T> TaskDelegate<T>(CancellationToken cancellationToken);
}
