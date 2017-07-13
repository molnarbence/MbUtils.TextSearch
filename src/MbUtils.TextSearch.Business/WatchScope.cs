using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MbUtils.TextSearch.Business
{
    /// <summary>
    /// A helper class to measure execution time
    /// </summary>
    public class WatchScope : IDisposable
    {
        readonly Action<long> callback;
        readonly Stopwatch sw;
        public WatchScope(Action<long> callback)
        {
            this.callback = callback;
            sw = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            sw.Stop();
            callback(sw.ElapsedMilliseconds);
        }
    }
}
