using Core;
using McMaster.Extensions.CommandLineUtils;

namespace ConsoleApp;

[Command(Name = "Text Search App", Description = "Search for text in files")]
public class TextSearchApp
{
    [Argument(0, Name = "Input folder path")] 
    public required string InputFolderPath { get; set; }
    
    [Argument(1, Name = "Search term")] 
    public required string SearchTerm { get; set; }
    
    private void OnExecute(
        MainLogic mainLogic, IConsole console)
    {
        var statistics = mainLogic.Search(InputFolderPath, SearchTerm);
        
        console.WriteLine($"               Buffer size: {statistics.BufferSize}");
        console.WriteLine($" Max degree of parallelism: {statistics.ParallelTasks}");
        console.WriteLine($"                  Strategy: {statistics.Strategy}");
        console.WriteLine($"          Total bytes read: {statistics.TotalBytesRead} ({statistics.TotalMegabytesRead:00.00} MB)");
        console.WriteLine($"        Total milliseconds: {statistics.TotalMilliseconds}");
        console.WriteLine($"            Avg. exec rate: {statistics.ReadRateInBytesPerSecond} bytes/s ({statistics.ReadRateInMegabytesPerSecond:0.00} MB/s)");
    }
}