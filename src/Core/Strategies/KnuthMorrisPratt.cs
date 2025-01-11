namespace Core.Strategies;

/// <summary>
/// String searching, using Knuth-Morris-Pratt algorithm
/// https://www.programmingalgorithms.com/algorithm/knuth%E2%80%93morris%E2%80%93pratt-algorithm
/// </summary>
public class KnuthMorrisPratt : ISearchTermCounterStrategy
{
    private static int[]? _lpsArray;

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

    public int Count(string input, string pattern)
    {
        _lpsArray ??= ComputeLpsArray(pattern, pattern.Length);

        var retVal = new List<int>();
        var patternLength = pattern.Length;
        var inputLength = input.Length;
        var i = 0;
        var j = 0;

        while (i < inputLength)
        {
            if (pattern[j] == input[i])
            {
                j++;
                i++;
            }

            if (j == patternLength)
            {
                retVal.Add(i - j);
                j = _lpsArray[j - 1];
            }

            else if (i < inputLength && pattern[j] != input[i])
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