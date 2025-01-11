using System.Text;
using Microsoft.Extensions.Options;

namespace Core;

[RegisterSingleton]
public class StreamInspector(IOptions<AppConfig> config, ISearchTermCounterStrategyFactory strategyFactory)
    : IStreamInspector
{
    private readonly Encoding _encoding = config.Value.IsUtf8 ? Encoding.UTF8 : Encoding.ASCII;

    // Total read bytes, for statistics
    private long _totalReadBytesCount;
    private void AddReadBytesCount(long count)
    {
        Interlocked.Add(ref _totalReadBytesCount, count);
    }
    public long TotalReadBytesCount => _totalReadBytesCount;

    public async Task<int> GetNumberOfMatchesAsync(string filePath, string searchTerm)
    {
        // variables to remember
        var ret = 0;
        var searchTermLength = searchTerm.Length;
        var buffer = new char[config.Value.BufferSize]; // to reuse buffer multiple times
        var strategy = strategyFactory.Create(searchTerm);

        await using var fs = File.OpenRead(filePath);
        using var sr = new StreamReader(fs, _encoding, true);

        var partialMatchFromPreviousChunk = string.Empty;

        while (await ReadNextChunkAsync(sr, buffer) is { } currentChunk)
        {
            // check if we have partial match from previous chunk
            if (!string.IsNullOrEmpty(partialMatchFromPreviousChunk))
            {
                // concatenate substring from the end of previous chunk,
                // and add substring from the beginning of current chunk
                var potentialMatch = string.Concat(partialMatchFromPreviousChunk, currentChunk[..(searchTermLength - partialMatchFromPreviousChunk.Length)]);

                // check if it's a match
                if (string.Compare(searchTerm, potentialMatch, StringComparison.OrdinalIgnoreCase) == 0)
                    ret++;

                // reset partial match
                partialMatchFromPreviousChunk = string.Empty;
            }

            // count matches
            var count = strategy.Count(currentChunk);

            // some variables to remember
            var chunkLength = currentChunk.Length;

            // need to check at the end of the chunk if we can find a partial match
            if(chunkLength > (searchTermLength - 1))
            {
                for (var i = 1; i < searchTermLength; i++)
                {
                    // construct substrings to compare
                    var subSearchTerm = searchTerm[..^i];
                    var subChunk = currentChunk[(chunkLength - subSearchTerm.Length)..];

                    // check if it's a match
                    if (string.Compare(subSearchTerm, subChunk, StringComparison.OrdinalIgnoreCase) != 0) continue;
                    
                    // found a match
                    partialMatchFromPreviousChunk = subChunk;
                    break;
                }
            }

            // increment match count
            ret += count;
        }

        // add read bytes count (for statistics)
        AddReadBytesCount(fs.Length);

        return ret;
    }

    private static async Task<string?> ReadNextChunkAsync(StreamReader reader, char[] buffer)
    {
        int readBytesCount;
        return (readBytesCount = await reader.ReadBlockAsync(buffer, 0, buffer.Length)) > 0 
            ? new string(buffer, 0, readBytesCount) 
            : null;
    }
}