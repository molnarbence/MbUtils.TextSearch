namespace Core;

public interface IStreamInspector
{
    Task<int> GetNumberOfMatchesAsync(string filePath, string searchTerm);
}