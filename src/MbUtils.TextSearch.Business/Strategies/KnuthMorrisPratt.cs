namespace MbUtils.TextSearch.Business.Strategies;

/// <summary>
/// String searching, using Knuth-Morris-Pratt algorithm
/// https://www.programmingalgorithms.com/algorithm/knuth%E2%80%93morris%E2%80%93pratt-algorithm
/// </summary>
public class KnuthMorrisPratt : ISearchTermCounterStrategy
{
    private readonly int[] _lpsArray;
    private readonly string _pattern;

    public KnuthMorrisPratt(string pattern)
    {
        if (string.IsNullOrEmpty(pattern))
            throw new ArgumentNullException(nameof(pattern));

        _pattern = pattern;
        _lpsArray = ComputeLpsArray(pattern, pattern.Length);
    }

    private static int[] ComputeLpsArray(string pat, int m)
    {
        var len = 0;
        var i = 1;
        var lps = new int[m];

        lps[0] = 0;

        while (i < m)
        {
            if (pat[i] == pat[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else
            {
                if (len != 0)
                {
                    len = lps[len - 1];
                }
                else
                {
                    lps[i] = 0;
                    i++;
                }
            }
        }

        return lps;
    }

    public int Count(string input)
    {
        var retVal = new List<int>();
        var patternLength = _pattern.Length;
        var inputLength = input.Length;
        var i = 0;
        var j = 0;

        while (i < inputLength)
        {
            if (_pattern[j] == input[i])
            {
                j++;
                i++;
            }

            if (j == patternLength)
            {
                retVal.Add(i - j);
                j = _lpsArray[j - 1];
            }

            else if (i < inputLength && _pattern[j] != input[i])
            {
                if (j != 0)
                    j = _lpsArray[j - 1];
                else
                    i += 1;
            }
        }

        return retVal.Count;
    }
}