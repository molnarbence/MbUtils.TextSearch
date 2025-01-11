namespace Core;

public interface IStreamInspector
{
    Task<int> GetNumberOfMatchesAsync(Stream stream, string searchTerm);
}