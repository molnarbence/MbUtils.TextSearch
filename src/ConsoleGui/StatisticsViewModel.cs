using System.Runtime.Serialization;
using Core;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace ConsoleGui;

[DataContract]
public partial class StatisticsViewModel : ReactiveObject
{
    [DataMember, Reactive] private int _bufferSize;
    [DataMember, Reactive] private int _parallelTasks;
    [DataMember, Reactive] private string _strategy = string.Empty;
    [DataMember, Reactive] private long _totalMilliseconds;
    [DataMember, Reactive] private long _totalBytesRead;
    [DataMember, Reactive] private double _totalMegabytesRead;
    [DataMember, Reactive] private long _readRateInBytesPerSecond;
    [DataMember, Reactive] private double _readRateInMegabytesPerSecond;
    
    public void UpdateStatistics(Statistics statistics)
    {
        BufferSize = statistics.BufferSize;
        ParallelTasks = statistics.ParallelTasks;
        Strategy = statistics.Strategy;
        TotalBytesRead = statistics.TotalBytesRead;
        TotalMilliseconds = statistics.TotalMilliseconds;
        TotalMegabytesRead = statistics.TotalMegabytesRead;
        ReadRateInBytesPerSecond = statistics.ReadRateInBytesPerSecond;
        ReadRateInMegabytesPerSecond = statistics.ReadRateInMegabytesPerSecond;
    }
}