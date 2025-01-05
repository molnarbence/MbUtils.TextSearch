using Microsoft.Extensions.Logging;

namespace MbUtils.TextSearch.Business;

public class FilePathProvider(ILoggerFactory loggerFactory) : IFilePathProvider
{
    private readonly ILogger<FilePathProvider> _logger = loggerFactory.CreateLogger<FilePathProvider>();

    public IEnumerable<string> GetFilePaths(string rootFolderPath)
    {
        // try to enumerate folder files
        IEnumerable<string> files = [];
        try
        {
            files = Directory.EnumerateFiles(rootFolderPath);
        }
        catch (Exception ex)
        {
            _logger.LogDebug("{RootFolderPath}: {Message}", rootFolderPath, ex.Message);
        }

        // return files if any
        foreach (var item in files)
        {
            yield return item;
        }

        // try enumerate folders
        IEnumerable<string> folders = [];
        try
        {
            folders = Directory.EnumerateDirectories(rootFolderPath);
        }
        catch (Exception ex)
        {
            _logger.LogDebug("{RootFolderPath}: Can't enumerate directories. {ExceptionMessage}", rootFolderPath, ex.Message);
        }

        foreach (var item in folders)
        {
            // recursive call to enumerate subfolder's files
            var subFiles = GetFilePaths(item);
            foreach (var subFile in subFiles)
            {
                yield return subFile;
            }
        }
    }
}