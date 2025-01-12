using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;

namespace Core;

public sealed class FileBasedResultRepository : IResultRepository, IDisposable
{
    private readonly IOptions<AppConfig> _config;
    private readonly ConcurrentQueue<SearchResult> _queue;
    private readonly AutoResetEvent _waitHandle;
    private readonly ILogger<FileBasedResultRepository> _logger;
    private readonly CancellationTokenSource _tokenSource;
    private readonly CancellationToken _token;

    public FileBasedResultRepository(ILogger<FileBasedResultRepository> logger, 
        IOptions<AppConfig> config)
    {
        _config = config;
        _logger = logger;
        
        // validate output file and folder
        var fileInfo = new FileInfo(config.Value.OutputFilePath);
        if (!fileInfo.Directory?.Exists ?? true)
            fileInfo.Directory?.Create();
        if (fileInfo.Exists)
            fileInfo.Delete();
        File.WriteAllText(config.Value.OutputFilePath, $"Path,Match count{Environment.NewLine}");

        _waitHandle = new AutoResetEvent(false);
        _queue = new ConcurrentQueue<SearchResult>();

        _tokenSource = new CancellationTokenSource();
        _token = _tokenSource.Token;
        Task.Run(SaveTask, _token);
    }

    /// <summary>
    /// Queue the result, so the background thread can write it to file
    /// </summary>
    /// <param name="result"></param>
    public void SaveResult(SearchResult result)
    {
        _queue.Enqueue(result);
        _waitHandle.Set();
    }

    /// <summary>
    /// Actually doing the saving the content to file, on a background thread
    /// </summary>
    private void SaveTask()
    {
        while(!_token.IsCancellationRequested)
        {
            // wait for the next signal
            _waitHandle.WaitOne();

            // dequeue one item
            while (_queue.TryDequeue(out var result))
            {
                try
                {
                    // save to file
                    DoSave(result);
                }
                catch (Exception ex)
                {
                    _logger.LogDebug("Could not save file. {ExceptionMessage}", ex.Message);
                }
            }
        }
            
    }

    private void DoSave(SearchResult result) 
        => File.AppendAllText(_config.Value.OutputFilePath, $"\"{result.FilePath}\",{result.MatchCount}{Environment.NewLine}");

    #region IDisposable Support
    private bool _disposedValue;
        
    public void Dispose()
    {
        if (_disposedValue) return;
        
        // dequeue all remaining
        while (_queue.TryDequeue(out var dummy))
        {
        }

        // cancel file writer thread
        try
        {
            _tokenSource.Cancel();
        }
        catch
        {
            // ignored
        }

        // set to disposed
        _disposedValue = true;
    }
    #endregion
}
