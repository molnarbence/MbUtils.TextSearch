using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace Core;

[RegisterSingleton]
public class MainLogic(
    IFilePathProvider filePathProvider,
    IStreamInspector streamInspector,
    IResultRepository resultRepo,
    IOptions<AppConfig> config)
{
    // Total read bytes, for statistics
    private long _totalReadBytesCount;
    private void AddReadBytesCount(long count)
    {
        Interlocked.Add(ref _totalReadBytesCount, count);
    }
    
    public Statistics Search(string inputFolderPath, string searchTerm)
    {
        var watch = Stopwatch.StartNew();
        
        // get file paths (as enumerable)
        var filePaths = filePathProvider.GetFilePaths(inputFolderPath);

        if (config.Value.ParallelTasks > 1)
        {
            Parallel.ForEach(filePaths, new ParallelOptions { MaxDegreeOfParallelism = config.Value.ParallelTasks }, (filePath) => {
                SearchAsync(filePath, searchTerm).Wait();
            });
        }
        else
        {
            var taskList = filePaths.Select(item => SearchAsync(item, searchTerm)).ToList();

            Task.WhenAll(taskList).Wait();
        }
        
        watch.Stop();

        return new Statistics(
            config.Value.BufferSize,
            config.Value.ParallelTasks,
            config.Value.Strategy,
            _totalReadBytesCount,
            watch.ElapsedMilliseconds
            );
    }

    private async Task SearchAsync(string filePath, string searchTerm)
    {   
        // search
        await using var fs = File.OpenRead(filePath);
        var streamLength = fs.Length;
        var matchCount = await streamInspector.GetNumberOfMatchesAsync(fs, searchTerm);
        // save the result
        resultRepo.SaveResult(new SearchResult(filePath, matchCount));
        
        // add read bytes count (for statistics)
        AddReadBytesCount(streamLength);
    }
}