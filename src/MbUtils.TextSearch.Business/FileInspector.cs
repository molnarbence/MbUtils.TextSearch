using MbUtils.TextSearch.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MbUtils.TextSearch.Business
{
    public class FileInspector : IFileInspector
    {
        readonly int bufferSize;
        readonly Encoding encoding;
        readonly ILogger<FileInspector> logger;
        object lockObj = new object();

        long totalReadBytesCount = 0;

        public void AddReadBytesCount(long count)
        {
            Interlocked.Add(ref totalReadBytesCount, count);
        }
        public long TotalReadBytesCount { get { return totalReadBytesCount; } }

        public FileInspector(ILoggerFactory loggerFactory, int bufferSize, bool isUtf8)
        {
            logger = loggerFactory.CreateLogger<FileInspector>();
            this.bufferSize = bufferSize;
            encoding = isUtf8 ? Encoding.UTF8 : Encoding.ASCII;
        }

        public async Task<int> GetNumberOfMatchesAsync(string filePath, string searchTerm)
        {
            var ret = 0;
            var searchTermLength = searchTerm.Length;
            var regex = new Regex(Regex.Escape(searchTerm));

            using (var fs = File.OpenRead(filePath))
            {
                var chunk = default(string);

                var partialMatchFromPreviousChunk = string.Empty;

                while ((chunk = await GetChunksAsStringAsync(fs)) != null)
                {
                    // check if we have partial match from previous chunk
                    if(!string.IsNullOrEmpty(partialMatchFromPreviousChunk))
                    {
                        // concatenate characters from the current chunk
                        var potentialMatch = string.Concat(partialMatchFromPreviousChunk, chunk.Substring(0, searchTermLength - partialMatchFromPreviousChunk.Length));

                        // check if it's a match
                        if (string.Compare(searchTerm, potentialMatch, true) == 0)
                            ret++;

                        // reset partial match
                        partialMatchFromPreviousChunk = string.Empty;
                    }

                    // some variables to remember
                    var count = regex.Matches(chunk).Count;
                    var chunkLength = chunk.Length;

                    // need to check at the end of the chunk if we can find a next match
                    for (int i = 1; i < searchTermLength; i++)
                    {
                        // construct substrings
                        var subSearchTerm = searchTerm.Substring(0, searchTerm.Length - i);
                        var subChunk = chunk.Substring(chunkLength - subSearchTerm.Length);

                        // check if it's a match
                        if(string.Compare(subSearchTerm, subChunk, true) == 0)
                        {
                            // found a match
                            partialMatchFromPreviousChunk = subChunk;
                            break;
                        }
                    }

                    // increment match count
                    ret += count;

                    // add read bytes count (for statistics)
                    AddReadBytesCount(chunkLength);
                }
               
            }

            return ret;
        }

        private async Task<string> GetChunksAsStringAsync(Stream inputStream)
        {
            // TODO: UTF-8 can be multibyte, so it can happen, that one or more byte won't be read at the end
            // We need to address this somehow
            var buffer = new byte[bufferSize];

            var readBytesCount = 0;
            if ((readBytesCount = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                return encoding.GetString(buffer, 0, readBytesCount);
            else
                return null;
        }
    }
}
