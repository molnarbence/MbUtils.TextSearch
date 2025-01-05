using MbUtils.TextSearch.Business;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace UnitTests;

public class MainLogicTests
{
    private const int Parallelism = 2;
    private const bool IsParallel = false;
    private const string InputFolderPath = "inputfolder";
    private const string SearchTerm = "target";
    
    private readonly IResultRepository _resultRepo = Substitute.For<IResultRepository>();
    
    private readonly List<ISearchTermCounterStrategy> _strategies =
    [
        new RegexStrategy(SearchTerm),
        new KnuthMorrisPratt(SearchTerm),
        new StringSplitStrategy(SearchTerm)
    ];
    
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [Theory]
    public void TestSearchStrategies(int strategyIndex)
    {
        // arrange 
        const int bufferSize = 1 * 1024 * 1024;
        var unitUnderTest = CreateMainLogic(strategyIndex, bufferSize);
        
        // act
        unitUnderTest.Search(InputFolderPath);

        // assert
        _resultRepo.Received(2).SaveResult(Arg.Any<SearchResult>());
        _resultRepo.Received(1).SaveResult(Arg.Is<SearchResult>(s => s.MatchCount == 6 && s.FilePath.EndsWith("file1.txt")));
        _resultRepo.Received(1).SaveResult(Arg.Is<SearchResult>(s => s.MatchCount == 11 && s.FilePath.EndsWith("file2.txt")));
    }

    [InlineData(32)]
    [InlineData(1024 * 1024)]
    [Theory]
    public void TestBufferSizes(int bufferSize)
    {
        // arrange
        const int strategyIndex = 0;
        var unitUnderTest = CreateMainLogic(strategyIndex, bufferSize);
        
        // act
        unitUnderTest.Search(InputFolderPath);
        
        // assert
        _resultRepo.Received(2).SaveResult(Arg.Any<SearchResult>());
        _resultRepo.Received(1).SaveResult(Arg.Is<SearchResult>(s => s.MatchCount == 6 && s.FilePath.EndsWith("file1.txt")));
        _resultRepo.Received(1).SaveResult(Arg.Is<SearchResult>(s => s.MatchCount == 11 && s.FilePath.EndsWith("file2.txt")));
    }
    
    private MainLogic CreateMainLogic(int strategyIndex, int bufferSize)
    {
        var loggerFactory = new LoggerFactory();
        var filePathProvider = new FilePathProvider(loggerFactory);
        var strategy = _strategies[strategyIndex];
        var fileInspector = new FileInspector(loggerFactory, bufferSize, true, SearchTerm, strategy);
        return new MainLogic(loggerFactory, filePathProvider, fileInspector, _resultRepo, Parallelism, IsParallel);
    }
}