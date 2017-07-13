using MbUtils.TextSearch.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MbUtils.TextSearch.Business
{
    public class FileInspector : IFileInspector
    {
        readonly int bufferSize;
        readonly Encoding encoding;
        readonly ILogger<FileInspector> logger;

        public long TotalReadBytesCount { get; private set; }

        public FileInspector(ILoggerFactory loggerFactory, int bufferSize, bool isUtf8)
        {
            logger = loggerFactory.CreateLogger<FileInspector>();
            this.bufferSize = bufferSize;
            encoding = isUtf8 ? Encoding.UTF8 : Encoding.ASCII;
        }

        public async Task<int> GetNumberOfMatchesAsync(string filePath, string searchTerm)
        {
            var ret = 0;
            var buffer = new byte[bufferSize];

            using (var fs = File.OpenRead(filePath))
            {
                var searchTermOffset = 0;
                var chunk = default(string);

                while ((chunk = await GetChunksAsStringAsync(fs)) != null)
                {
                    // iterate through the characters of the chunk
                    for (int chunkOffset = 0; chunkOffset < chunk.Length; chunkOffset++)
                    {
                        // check if characters match
                        if (chunk[chunkOffset] == searchTerm[searchTermOffset])
                        {
                            // characters match, so let's increment the pointer of the searchterm
                            searchTermOffset++;

                            // check if we reached the end of searchterm
                            if (searchTermOffset == searchTerm.Length)
                            {
                                // we found a match, so increment the count
                                ret++;
                                // set the searchterm pointer back to the beginning
                                searchTermOffset = 0;
                            }
                        }
                        else
                        {
                            // don't match so set the searchterm pointer back to the beginning
                            searchTermOffset = 0;
                        }
                    }
                    TotalReadBytesCount += chunk.Length;
                }
               
            }

            return ret;
        }

        private async Task<string> GetChunksAsStringAsync(Stream inputStream)
        {
            var buffer = new byte[bufferSize];

            var readBytesCount = 0;
            if ((readBytesCount = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                return encoding.GetString(buffer, 0, readBytesCount);
            else
                return null;
        }
    }
}
