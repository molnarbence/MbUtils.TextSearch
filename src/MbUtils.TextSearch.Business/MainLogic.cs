using MbUtils.TextSearch.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MbUtils.TextSearch.Business
{
    public class MainLogic
    {
        readonly ILogger<MainLogic> logger;
        readonly IFilePathProvider filePathProvider;
        readonly IFileInspector fileInspector;
        readonly IResultRepository resultRepo;
             

        public MainLogic(
            ILoggerFactory loggerFactory, 
            IFilePathProvider filePathProvider, 
            IFileInspector fileInspector,
            IResultRepository resultRepo)
        {
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

            Parallel.ForEach(filePaths, new ParallelOptions() { MaxDegreeOfParallelism = 2 }, (filePath) => {
                var matchCount = fileInspector.GetNumberOfMatchesAsync(filePath, searchTerm).Result;
                resultRepo.SaveResult(new SearchResult { FilePath = filePath, MatchCount = matchCount });
            });
        }

        #region Input validation
        private void ValidateInputFolderPath(string inputFolderPath)
        {
            var inputFolderInfo = default(DirectoryInfo);
            try
            {
                inputFolderInfo = new DirectoryInfo(inputFolderPath);
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex.Message);
                throw new ArgumentException($"Invalid input folder path {inputFolderPath}");
            }
            if (!inputFolderInfo.Exists)
                throw new ArgumentException($"Input folder {inputFolderPath} doesn't exist", nameof(inputFolderPath));
        }
        #endregion
    }
}
