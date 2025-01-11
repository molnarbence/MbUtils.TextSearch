using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Options;

namespace Core;

[RegisterSingleton]
public class MainLogic(
    IFilePathProvider filePathProvider,
    ISearchTermCounterStrategy strategy,
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
                SearchAsync(filePath, searchTerm);
            });
        }
        else
        {
            foreach (var filePath in filePaths)
            {
                SearchAsync(filePath, searchTerm);
            }
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

    private void SearchAsync(string filePath, string searchTerm)
    {   
        // search
        using var fs = File.OpenRead(filePath);
        var streamLength = fs.Length;
        var encoding = config.Value.IsUtf8 ? Encoding.UTF8 : Encoding.ASCII;
        var matchCount = strategy.GetNumberOfMatches(fs, searchTerm, config.Value.BufferSize, encoding);
        // save the result
        resultRepo.SaveResult(new SearchResult(filePath, matchCount));
        
        // add read bytes count (for statistics)
        AddReadBytesCount(streamLength);
    }
}