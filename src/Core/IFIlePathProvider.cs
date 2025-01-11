namespace Core;

public interface IFilePathProvider
{
    IEnumerable<string> GetFilePaths(string rootFolderPath);
}