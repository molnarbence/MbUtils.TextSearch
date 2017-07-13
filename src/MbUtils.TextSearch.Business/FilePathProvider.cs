using MbUtils.TextSearch.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MbUtils.TextSearch.Business
{
    public class FilePathProvider : IFilePathProvider
    {
        readonly ILogger<FilePathProvider> logger;

        public FilePathProvider(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<FilePathProvider>();
        }

        public IEnumerable<string> GetFilePaths(string rootFolderPath)
        {
            // try to get enumerator for the folder files
            var files = default(IEnumerable<string>);
            try
            {
                files = Directory.EnumerateFiles(rootFolderPath);
            }
            catch (Exception ex)
            {
                logger.LogDebug($"{rootFolderPath}: {ex.Message}");
            }

            // return files if any
            if (files != null)
            {
                foreach (var item in files)
                {
                    yield return item;
                }
            }

            // get enumerator for directories
            var folders = Directory.EnumerateDirectories(rootFolderPath);
            foreach (var item in folders)
            {
                var subFiles = GetFilePaths(item);
                foreach (var subFile in subFiles)
                {
                    yield return subFile;
                }
            }
        }
    }
}
