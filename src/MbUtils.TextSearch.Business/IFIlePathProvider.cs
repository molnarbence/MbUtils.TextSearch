namespace MbUtils.TextSearch.Business
{
    public interface IFilePathProvider
    {
        IEnumerable<string> GetFilePaths(string rootFolderPath);
    }
}
