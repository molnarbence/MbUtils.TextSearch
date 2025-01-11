namespace Core;

public record Statistics(
    int BufferSize,
    int ParallelTasks,
    string Strategy,
    long TotalBytesRead,
    long TotalMilliseconds
)
{
    public double TotalMegabytesRead { get; } = (double)TotalBytesRead / (1000 * 1000);
    public long ReadRateInBytesPerSecond { get; } = (TotalBytesRead * 1000) / Math.Max(1, TotalMilliseconds);
    public double ReadRateInMegabytesPerSecond { get; } = (double)TotalBytesRead / Math.Max(1, TotalMilliseconds) / 1000;

}