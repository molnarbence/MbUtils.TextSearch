namespace Core;

public static class FilePathProvider
{
    public static IEnumerable<string> GetFilePaths(string rootFolderPath)
    {
        // try to enumerate folder files
        var files = Directory.EnumerateFiles(rootFolderPath);

        // return files if any
        foreach (var item in files)
        {
            yield return item;
        }

        // try enumerate folders
        var folders = Directory.EnumerateDirectories(rootFolderPath);

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