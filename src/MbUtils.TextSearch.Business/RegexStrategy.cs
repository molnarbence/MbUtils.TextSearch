using System.Text.RegularExpressions;

namespace MbUtils.TextSearch.Business;

public class RegexStrategy(string pattern) : ISearchTermCounterStrategy
{
    private readonly Regex _regex = new(Regex.Escape(pattern));

    public int Count(string input) => _regex.Matches(input).Count;
}