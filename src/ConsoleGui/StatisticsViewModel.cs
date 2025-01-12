using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace ConsoleGui;

[DataContract]
public partial class StatisticsViewModel : ReactiveObject
{
    [DataMember]
    [Reactive] private int _bufferSize;
}