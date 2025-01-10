namespace MbUtils.TextSearch.Business;

public interface ISearchTermCounterStrategyFactory
{
    ISearchTermCounterStrategy Create(string searchTerm);
}