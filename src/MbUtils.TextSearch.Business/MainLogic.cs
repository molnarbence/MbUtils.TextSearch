using Microsoft.Extensions.Logging;

namespace MbUtils.TextSearch.Business;

public class MainLogic(
    ILoggerFactory loggerFactory,
    IFilePathProvider filePathProvider,
    IFileInspector fileInspector,
    IResultRepository resultRepo,
    int parallelism,
    bool isParallel)
{
    // injected services
    private readonly ILogger<MainLogic> _logger = loggerFactory.CreateLogger<MainLogic>();

    // configuration values

    public void Search(string inputFolderPath, string searchTerm)
    {
        // validate input
        ValidateInputFolderPath(inputFolderPath);

        // get file paths (as enumerable)
        var filePaths = filePathProvider.GetFilePaths(inputFolderPath);

        if (isParallel)
        {
            Parallel.ForEach(filePaths, new ParallelOptions { MaxDegreeOfParallelism = parallelism }, (filePath) => {
                DoSearch(filePath, searchTerm).Wait();
            });
        }
        else
        {
            var taskList = new List<Task>();
            foreach (var item in filePaths)
            {
                taskList.Add(DoSearch(item, searchTerm));
            }

            Task.WhenAll(taskList).Wait();
        }

    }

    private async Task DoSearch(string filePath, string searchTerm)
    {   
        try
        {
            // search
            var matchCount = await fileInspector.GetNumberOfMatchesAsync(filePath, searchTerm);
            // save the result
            resultRepo.SaveResult(new SearchResult(filePath, matchCount));
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not perform search, {ExceptionMessage}", ex.Message);
        }
    }

    #region Input validation
    private void ValidateInputFolderPath(string inputFolderPath)
    {
        // check if it's null
        if (string.IsNullOrEmpty(inputFolderPath))
            throw new ArgumentNullException(nameof(inputFolderPath));

        DirectoryInfo inputFolderInfo;
        try
        {
            // check if path is in valid format
            inputFolderInfo = new DirectoryInfo(inputFolderPath);
        }
        catch (Exception ex)
        {
            _logger.LogDebug("Input folder path '{InputFolderPath}' was not in the correct format.  {ExceptionMessage}", inputFolderPath, ex.Message);
            throw new ArgumentException($"Invalid input folder path {inputFolderPath}");
        }

        // check if folder exists
        if (!inputFolderInfo.Exists)
            throw new ArgumentException($"Input folder {inputFolderPath} doesn't exist", nameof(inputFolderPath));
    }
    #endregion
}