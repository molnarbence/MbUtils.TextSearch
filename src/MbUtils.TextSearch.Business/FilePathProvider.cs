using Microsoft.Extensions.Logging;

namespace MbUtils.TextSearch.Business;

[RegisterSingleton]
public class FilePathProvider(ILogger<FilePathProvider> logger) : IFilePathProvider
{
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
            logger.LogDebug("{RootFolderPath}: {Message}", rootFolderPath, ex.Message);
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
            logger.LogDebug("{RootFolderPath}: Can't enumerate directories. {ExceptionMessage}", rootFolderPath, ex.Message);
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