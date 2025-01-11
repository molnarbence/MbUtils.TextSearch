namespace Core;

public interface IFileInspector
{
    Task<int> GetNumberOfMatchesAsync(string filePath, string searchTerm);
}