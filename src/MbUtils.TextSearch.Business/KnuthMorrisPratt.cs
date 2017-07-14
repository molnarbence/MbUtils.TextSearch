using MbUtils.TextSearch.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MbUtils.TextSearch.Business
{
    public class KnuthMorrisPratt : ISearchTermCounterStrategy
    {
        readonly int[] lpsArray;
        readonly string pattern;

        public KnuthMorrisPratt(string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                throw new ArgumentNullException(nameof(pattern));

            this.pattern = pattern;
            lpsArray = ComputeLpsArray(pattern, pattern.Length);
        }

        private int[] ComputeLpsArray(string pat, int m)
        {
            int len = 0;
            int i = 1;
            int[] lps = new int[m];

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
            var pat = pattern;
            List<int> retVal = new List<int>();
            int M = pat.Length;
            int N = input.Length;
            int i = 0;
            int j = 0;

            while (i < N)
            {
                if (pat[j] == input[i])
                {
                    j++;
                    i++;
                }

                if (j == M)
                {
                    retVal.Add(i - j);
                    j = lpsArray[j - 1];
                }

                else if (i < N && pat[j] != input[i])
                {
                    if (j != 0)
                        j = lpsArray[j - 1];
                    else
                        i = i + 1;
                }
            }

            return retVal.Count;
        }
    }
}
