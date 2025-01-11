using System.Text.RegularExpressions;

namespace Core.Strategies;

public class RegexStrategy : ISearchTermCounterStrategy
{
    public int Count(string input, string pattern) => Regex.Matches(input, Regex.Escape(pattern)).Count;
}