﻿using Microsoft.Extensions.Logging;

namespace MbUtils.TextSearch.Business
{
    public class MainLogic
    {
        // injected services
        readonly ILogger<MainLogic> logger;
        readonly IFilePathProvider filePathProvider;
        readonly IFileInspector fileInspector;
        readonly IResultRepository resultRepo;

        readonly bool isParallel;

        // configuration values
        readonly int parallelism = 2;

        public MainLogic(
            ILoggerFactory loggerFactory, 
            IFilePathProvider filePathProvider, 
            IFileInspector fileInspector,
            IResultRepository resultRepo,
            int parallelism,
            bool isParallel)
        {
            this.parallelism = parallelism;
            this.isParallel = isParallel;
            logger = loggerFactory.CreateLogger<MainLogic>();
            this.filePathProvider = filePathProvider;
            this.fileInspector = fileInspector;
            this.resultRepo = resultRepo;
        }

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
                logger.LogError("Could not perform search, {ExceptionMessage}", ex.Message);
            }
        }

        #region Input validation
        private void ValidateInputFolderPath(string inputFolderPath)
        {
            // check if it's null
            if (string.IsNullOrEmpty(inputFolderPath))
                throw new ArgumentNullException(nameof(inputFolderPath));

            var inputFolderInfo = default(DirectoryInfo);
            try
            {
                // check if path is in valid format
                inputFolderInfo = new DirectoryInfo(inputFolderPath);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex.Message);
                throw new ArgumentException($"Invalid input folder path {inputFolderPath}");
            }

            // check if folder exists
            if (!inputFolderInfo.Exists)
                throw new ArgumentException($"Input folder {inputFolderPath} doesn't exist", nameof(inputFolderPath));
        }
        #endregion
    }
}
