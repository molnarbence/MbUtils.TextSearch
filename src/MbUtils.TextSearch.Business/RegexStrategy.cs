using System.Text.RegularExpressions;

namespace MbUtils.TextSearch.Business
{
    public class RegexStrategy : ISearchTermCounterStrategy
    {
        readonly Regex regex;
        public RegexStrategy(string pattern)
        {
            regex = new Regex(Regex.Escape(pattern));
        }

        public int Count(string input)
        {
            return regex.Matches(input).Count;
        }
    }
}
