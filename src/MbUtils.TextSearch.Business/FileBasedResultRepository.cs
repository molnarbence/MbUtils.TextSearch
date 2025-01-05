using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace MbUtils.TextSearch.Business
{
    public class FileBasedResultRepository : IResultRepository, IDisposable
    {
        readonly string outputFilePath;
        readonly ConcurrentQueue<SearchResult> queue;
        readonly AutoResetEvent waitHandle;
        readonly ILogger<FileBasedResultRepository> logger;
        readonly CancellationTokenSource tokenSource;
        readonly CancellationToken token;

        public FileBasedResultRepository(ILoggerFactory loggerFactory, string outputFilePath)
        {
            // validate output file and folder
            try
            {
                var fileInfo = new FileInfo(outputFilePath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();
                if (fileInfo.Exists)
                    fileInfo.Delete();
                File.WriteAllText(outputFilePath, $"Path,Match count{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex.Message);
                throw new ArgumentException("Output file cannot be written");
            }

            logger = loggerFactory.CreateLogger<FileBasedResultRepository>();
            waitHandle = new AutoResetEvent(false);
            this.outputFilePath = outputFilePath;
            queue = new ConcurrentQueue<SearchResult>();

            tokenSource = new CancellationTokenSource();
            token = tokenSource.Token;
            Task.Run(() => SaveTask(), token);
        }

        /// <summary>
        /// Queue the result, so the background thread can write it to file
        /// </summary>
        /// <param name="result"></param>
        public void SaveResult(SearchResult result)
        {
            queue.Enqueue(result);
            waitHandle.Set();
        }

        /// <summary>
        /// Actually doing the saving the content to file, on a background thread
        /// </summary>
        private void SaveTask()
        {
            while(!token.IsCancellationRequested)
            {
                // wait for the next signal
                waitHandle.WaitOne();

                // dequeue one item
                SearchResult result;
                while (queue.TryDequeue(out result))
                {
                    if (result == null)
                        continue;

                    try
                    {
                        // save to file
                        DoSave(result);
                    }
                    catch (Exception ex)
                    {
                        logger.LogDebug(ex.Message);
                    }
                }
            }
            
        }

        private void DoSave(SearchResult result)
        {
            File.AppendAllText(outputFilePath, $"\"{result.FilePath}\",{result.MatchCount}{Environment.NewLine}");
        }

        #region IDisposable Support
        private bool disposedValue = false;
        
        public void Dispose()
        {
            if (!disposedValue)
            {   
                // dequeue all remaining
                SearchResult dummy;
                while (queue.TryDequeue(out dummy))
                {
                }

                // cancel file writer thread
                try
                {
                    tokenSource.Cancel();
                }
                catch { }

                // set to disposed
                disposedValue = true;
            }
        }
        #endregion
    }
}
