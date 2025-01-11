using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core;

[RegisterSingleton]
public class MainLogic(
    IFilePathProvider filePathProvider,
    IStreamInspector streamInspector,
    IResultRepository resultRepo,
    IOptions<AppConfig> config)
{

    public void Search(string inputFolderPath, string searchTerm)
    {
        // get file paths (as enumerable)
        var filePaths = filePathProvider.GetFilePaths(inputFolderPath);

        if (config.Value.ParallelTasks > 1)
        {
            Parallel.ForEach(filePaths, new ParallelOptions { MaxDegreeOfParallelism = config.Value.ParallelTasks }, (filePath) => {
                SearchAsync(filePath, searchTerm).Wait();
            });
        }
        else
        {
            var taskList = filePaths.Select(item => SearchAsync(item, searchTerm)).ToList();

            Task.WhenAll(taskList).Wait();
        }
    }

    private async Task SearchAsync(string filePath, string searchTerm)
    {   
        // search
        await using var fs = File.OpenRead(filePath);
        var matchCount = await streamInspector.GetNumberOfMatchesAsync(fs, searchTerm);
        // save the result
        resultRepo.SaveResult(new SearchResult(filePath, matchCount));
    }
}