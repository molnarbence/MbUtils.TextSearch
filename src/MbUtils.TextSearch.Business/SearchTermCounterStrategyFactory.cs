using MbUtils.TextSearch.Business.Strategies;
using Microsoft.Extensions.Options;

namespace MbUtils.TextSearch.Business;

[RegisterSingleton]
public class SearchTermCounterStrategyFactory(IOptions<AppConfig> config) : ISearchTermCounterStrategyFactory
{
    public ISearchTermCounterStrategy Create(string searchTerm) =>
        config.Value.Strategy switch {
            "Regex" => new RegexStrategy(searchTerm),
            "KnuthMorrisPratt" => new KnuthMorrisPratt(searchTerm),
            "StringSplit" => new StringSplitStrategy(searchTerm),
            _ => throw new NotSupportedException($"Strategy {config.Value.Strategy} is not supported.")
        };
}