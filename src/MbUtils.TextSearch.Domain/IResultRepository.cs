using System;
using System.Collections.Generic;
using System.Text;

namespace MbUtils.TextSearch.Domain
{
    public interface IResultRepository
    {
        void SaveResult(SearchResult result);
    }
}
