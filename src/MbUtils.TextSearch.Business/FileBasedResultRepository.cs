using MbUtils.TextSearch.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MbUtils.TextSearch.Business
{
    public class FileBasedResultRepository : IResultRepository, IDisposable
    {
        readonly string outputFilePath;
        readonly ConcurrentQueue<SearchResult> queue;
        readonly AutoResetEvent waitHandle;
        readonly ILogger<FileBasedResultRepository> logger;
        readonly Task saveTask;
        readonly CancellationTokenSource tokenSource;
        readonly CancellationToken token;

        public FileBasedResultRepository(ILoggerFactory loggerFactory, string outputFilePath)
        {
            // validate
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
            saveTask = Task.Run(() => SaveThreadMain(), token);
        }

        public void SaveResult(SearchResult result)
        {
            queue.Enqueue(result);
            waitHandle.Set();
        }

        private void SaveThreadMain()
        {
            while(!token.IsCancellationRequested)
            {
                waitHandle.WaitOne();

                SearchResult result;
                while (queue.TryDequeue(out result))
                {
                    if (result == null)
                        continue;

                    try
                    {
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
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    SearchResult dummy;
                    while(queue.TryDequeue(out dummy))
                    {
                    }
                    try
                    {
                        tokenSource.Cancel();
                    }
                    catch
                    {
                        
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~FileBasedResultRepository() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
