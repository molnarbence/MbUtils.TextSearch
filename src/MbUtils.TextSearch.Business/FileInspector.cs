using Microsoft.Extensions.Logging;
using System.Text;

namespace MbUtils.TextSearch.Business
{
    public class FileInspector : IFileInspector
    {
        readonly int bufferSize;
        readonly Encoding encoding;
        readonly ILogger<FileInspector> logger;
        readonly string searchTerm;
        readonly ISearchTermCounterStrategy strategy;
        
        // Total read bytes, for statistics
        long totalReadBytesCount = 0;
        private void AddReadBytesCount(long count)
        {
            Interlocked.Add(ref totalReadBytesCount, count);
        }
        public long TotalReadBytesCount { get { return totalReadBytesCount; } }

        public FileInspector(ILoggerFactory loggerFactory, int bufferSize, bool isUtf8, string searchTerm, ISearchTermCounterStrategy strategy)
        {
            logger = loggerFactory.CreateLogger<FileInspector>();

            this.bufferSize = bufferSize;
            encoding = isUtf8 ? Encoding.UTF8 : Encoding.ASCII;

            this.searchTerm = searchTerm;
            this.strategy = strategy;
        }

        public async Task<int> GetNumberOfMatchesAsync(string filePath)
        {
            // variables to remember
            var ret = 0;
            var searchTermLength = searchTerm.Length;
            var buffer = new char[bufferSize]; // to reuse buffer multiple times

            using (var fs = File.OpenRead(filePath))
            {
                using (var sr = new StreamReader(fs, encoding, true))
                {
                    var currentChunk = default(string);
                    var partialMatchFromPreviousChunk = default(string);

                    while ((currentChunk = await ReadNextChunkAsync(sr, buffer)) != null)
                    {
                        // check if we have partial match from previous chunk
                        if (!string.IsNullOrEmpty(partialMatchFromPreviousChunk))
                        {
                            // concatenate substring from the end of previous chunk,
                            // and add substring from the beginning of current chunk
                            var potentialMatch = string.Concat(partialMatchFromPreviousChunk, currentChunk.Substring(0, searchTermLength - partialMatchFromPreviousChunk.Length));

                            // check if it's a match
                            if (string.Compare(searchTerm, potentialMatch, true) == 0)
                                ret++;

                            // reset partial match
                            partialMatchFromPreviousChunk = null;
                        }

                        // count matches
                        var count = strategy.Count(currentChunk);

                        // some variables to remember
                        var chunkLength = currentChunk.Length;

                        // need to check at the end of the chunk if we can find a partial match
                        if(chunkLength > (searchTermLength - 1))
                        {
                            for (int i = 1; i < searchTermLength; i++)
                            {
                                // construct substrings to compare
                                var subSearchTerm = searchTerm.Substring(0, searchTerm.Length - i);
                                var subChunk = currentChunk.Substring(chunkLength - subSearchTerm.Length);

                                // check if it's a match
                                if (string.Compare(subSearchTerm, subChunk, true) == 0)
                                {
                                    // found a match
                                    partialMatchFromPreviousChunk = subChunk;
                                    break;
                                }
                            }
                        }

                        // increment match count
                        ret += count;
                    }

                    // add read bytes count (for statistics)
                    AddReadBytesCount(fs.Length);
                }
            }

            return ret;
        }

        private async Task<string> ReadNextChunkAsync(StreamReader reader, char[] buffer)
        {
            var readBytesCount = 0;
            if ((readBytesCount = await reader.ReadBlockAsync(buffer, 0, buffer.Length)) > 0)
                return new string(buffer, 0, readBytesCount);
            else
                return null;
        }
    }
}
