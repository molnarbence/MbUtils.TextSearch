namespace MbUtils.TextSearch.Business
{
    public interface IFileInspector
    {
        Task<int> GetNumberOfMatchesAsync(string filePath);
    }
}
