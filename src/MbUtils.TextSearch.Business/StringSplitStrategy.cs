using MbUtils.TextSearch.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace MbUtils.TextSearch.Business
{
    public class StringSplitStrategy : ISearchTermCounterStrategy
    {
        readonly string pattern;

        public StringSplitStrategy(string pattern)
        {
            this.pattern = pattern;
        }

        public int Count(string input)
        {
            return input.Split(new[] { pattern }, StringSplitOptions.None).Length - 1;
        }
    }
}
