using System;
using System.Collections.Generic;
using System.Text;

namespace MbUtils.TextSearch.Domain
{
    public interface ISearchTermCounterStrategy
    {
        int Count(string input);
    }
}
