namespace Core;

public interface ISearchTermCounterStrategyFactory
{
    ISearchTermCounterStrategy Create(string searchTerm);
}