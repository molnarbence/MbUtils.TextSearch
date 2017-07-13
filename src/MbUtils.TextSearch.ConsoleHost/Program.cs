using MbUtils.TextSearch.Business;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace MbUtils.TextSearch.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // check count of arguments, should be 3 or more
            if (args.Length < 3)
            {
                PrintUsage("Not enough input parameters");
                return;
            }

            // get the input parameters from command line
            var inputFolderPath = args[0];
            var searchTerm = args[1];
            var outputFilePath = args[2];

            const int bufferSize = 2 * 1024 * 1024;
            
            try
            {
                // wire up the application. In a bigger environment this part would be the DI container setup
                var loggerFactory = new LoggerFactory();
                loggerFactory.AddConsole(LogLevel.Debug);
                var filePathProvider = new FilePathProvider(loggerFactory);
                var fileInspector = new FileInspector(loggerFactory, bufferSize, true);
                var resultRepo = new FileBasedResultRepository(loggerFactory, outputFilePath);
                var mainLogic = new MainLogic(loggerFactory, filePathProvider, fileInspector, resultRepo);

                // call the search
                var totalMilliseconds = 0L;
                using (var scope = new WatchScope((ms) => totalMilliseconds = ms))
                {
                    mainLogic.Search(inputFolderPath, searchTerm);
                }

                // write some statistics
                var totalBytesRead = (double)fileInspector.TotalReadBytesCount;
                var totalMBRead = totalBytesRead / (1000 * 1000);
                var readRate = (totalBytesRead * 1000) / Math.Max(1, totalMilliseconds);
                var readRateInMB = readRate / (1000 * 1000);
                Console.WriteLine($"Buffer size: {bufferSize}");
                Console.WriteLine($"Total bytes read: {totalBytesRead} ({totalMBRead:00.00} MB)");
                Console.WriteLine($"Total milliseconds: {totalMilliseconds}");
                Console.WriteLine($"Avg. read: {readRate:0.00} bytes/s ({readRateInMB:0.00} MB/s)");
                
                resultRepo.Dispose();
            }
            catch (ArgumentException ex)
            {
                PrintUsage(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Done...");
        }

        static void PrintUsage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Correct usage is: [source path] [search term] [output file]");
            Console.WriteLine("e.g.: c:\\InputFolder \".net rocks\" c:\\OutFolder\\output.csv");
        }
    }
}
