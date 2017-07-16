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
            // try to enumerate folder files
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

            // try enumerate folders
            var folders = default(IEnumerable<string>);
            try
            {
                folders = Directory.EnumerateDirectories(rootFolderPath);
                
            }
            catch (Exception ex)
            {
                logger.LogDebug($"{rootFolderPath}: Can't enumerate directories. {ex.Message}");
            }
                
            if(folders != null)
            {
                foreach (var item in folders)
                {
                    // recursive call to enumerate subfolder's files
                    var subFiles = GetFilePaths(item);
                    foreach (var subFile in subFiles)
                    {
                        yield return subFile;
                    }
                }
            }
        }
    }
}
