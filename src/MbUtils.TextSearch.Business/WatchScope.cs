using System.Diagnostics;

namespace MbUtils.TextSearch.Business;

/// <summary>
/// A helper class to measure execution time
/// </summary>
public sealed class WatchScope(Action<long> callback) : IDisposable
{
    private readonly Stopwatch _sw = Stopwatch.StartNew();

    public void Dispose()
    {
        _sw.Stop();
        callback(_sw.ElapsedMilliseconds);
    }
}