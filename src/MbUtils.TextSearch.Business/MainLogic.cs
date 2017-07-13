using MbUtils.TextSearch.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MbUtils.TextSearch.Business
{
    public class MainLogic
    {
        readonly ILogger<MainLogic> logger;
        readonly IFilePathProvider filePathProvider;

        public MainLogic(ILoggerFactory loggerFactory, IFilePathProvider filePathProvider)
        {
            logger = loggerFactory.CreateLogger<MainLogic>();
            this.filePathProvider = filePathProvider;
        }

        public void Search(string inputFolderPath, string searchTerm, string outputFilePath)
        {
            // validate input
            ValidateInputFolderPath(inputFolderPath);
            ValidateOutputFile(outputFilePath);

            // get file paths (as enumerable)
            var filePaths = filePathProvider.GetFilePaths(inputFolderPath);

            foreach (var item in filePaths)
            {
                logger.LogInformation(item);
            }
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

        private void ValidateOutputFile(string outputFilePath)
        {
            try
            {
                File.WriteAllText(outputFilePath, "test");
            }
            catch (Exception ex)
            {
                logger.LogDebug(ex.Message);
                throw new ArgumentException("Output file cannot be written");
            }
        }
        #endregion
    }
}
