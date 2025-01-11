using Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace UnitTests;

public class MainLogicTests
{
    private const int Parallelism = 2;
    private const string InputFolderPath = "inputfolder";
    private const string SearchTerm = "target";

    private readonly IResultRepository _resultRepo = Substitute.For<IResultRepository>();

    [InlineData("Regex")]
    [InlineData("KnuthMorrisPratt")]
    [InlineData("StringSplit")]
    [Theory]
    public void TestSearchStrategies(string strategy)
    {
        // arrange 
        const int bufferSize = 1 * 1024 * 1024;
        var unitUnderTest = CreateMainLogic(strategy, bufferSize);

        // act
        unitUnderTest.Search(InputFolderPath, SearchTerm);

        // assert
        _resultRepo.Received(2).SaveResult(Arg.Any<SearchResult>());
        _resultRepo.Received(1)
            .SaveResult(Arg.Is<SearchResult>(s => s.MatchCount == 6 && s.FilePath.EndsWith("file1.txt")));
        _resultRepo.Received(1)
            .SaveResult(Arg.Is<SearchResult>(s => s.MatchCount == 11 && s.FilePath.EndsWith("file2.txt")));
    }

    [InlineData(32)]
    [InlineData(1024 * 1024)]
    [Theory]
    public void TestBufferSizes(int bufferSize)
    {
        // arrange
        var unitUnderTest = CreateMainLogic("Regex", bufferSize);

        // act
        unitUnderTest.Search(InputFolderPath, SearchTerm);

        // assert
        _resultRepo.Received(2).SaveResult(Arg.Any<SearchResult>());
        _resultRepo.Received().SaveResult(new SearchResult(Path.Combine(InputFolderPath, "file1.txt"), 6));
        _resultRepo.Received().SaveResult(new SearchResult(Path.Combine(InputFolderPath, "file2.txt"), 11));
    }

    private MainLogic CreateMainLogic(string strategy, int bufferSize)
    {
        var appConfig = Options.Create(new AppConfig
        {
            ParallelTasks = Parallelism,
            IsUtf8 = true,
            BufferSize = bufferSize,
            OutputFilePath = "",
            Strategy = strategy
        });

        var loggerFactory = new LoggerFactory();
        var filePathProvider = new FilePathProvider();
        var strategyFactory = new SearchTermCounterStrategyFactory(appConfig);

        var fileInspector = new StreamInspector(appConfig, strategyFactory);


        return new MainLogic(filePathProvider, fileInspector, _resultRepo, appConfig);
    }
}