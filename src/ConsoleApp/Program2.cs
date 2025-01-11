using Core;
using Core.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ConsoleApp;

class Program2
{
    const int BUFFERSIZE = 1 * 1024 * 1024;
    const int PARALLELISM = 2;
    const int STRATEGY = 1;

    public static void Main2(string[] args)
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
            
        try
        {
            // wire up the application. In a bigger environment this part would be the DI container setup
            var loggerFactory = new LoggerFactory();
            var filePathLogger = new Logger<FilePathProvider>(loggerFactory);
            var filePathProvider = new FilePathProvider(filePathLogger);


            // a bit more setup of services
            var appConfig = Options.Create(new AppConfig
            {
                IsUtf8 = true,
                BufferSize = BUFFERSIZE,
                OutputFilePath = outputFilePath,
                ParallelTasks = PARALLELISM
            });
            
            var strategyFactory = new SearchTermCounterStrategyFactory(appConfig);
            
            var fileInspector = new StreamInspector(appConfig, strategyFactory);

            var fileBasedResultRepositoryLogger = new Logger<FileBasedResultRepository>(loggerFactory);
            var resultRepo = new FileBasedResultRepository(fileBasedResultRepositoryLogger, appConfig);

            var mainLogic = new MainLogic(loggerFactory, filePathProvider, fileInspector, resultRepo, appConfig);

            // call the search, and measure the execution time
            var totalMilliseconds = 0L;
            using (new WatchScope((ms) => totalMilliseconds = ms))
            {
                mainLogic.Search(inputFolderPath, searchTerm);
            }

            // write some statistics
            var totalBytesRead = (double)fileInspector.TotalReadBytesCount;
            var totalMbRead = totalBytesRead / (1000 * 1000);
            var readRate = (totalBytesRead * 1000) / Math.Max(1, totalMilliseconds);
            var readRateInMb = readRate / (1000 * 1000);
            Console.WriteLine($"               Buffer size: {BUFFERSIZE}");
            Console.WriteLine($" Max degree of parallelism: {PARALLELISM}");
            Console.WriteLine($"                  Strategy: {appConfig.Value.Strategy}");
            Console.WriteLine($"          Total bytes read: {totalBytesRead} ({totalMbRead:00.00} MB)");
            Console.WriteLine($"        Total milliseconds: {totalMilliseconds}");
            Console.WriteLine($"            Avg. exec time: {readRate:0.00} bytes/s ({readRateInMb:0.00} MB/s)");
                
            // dispose services
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