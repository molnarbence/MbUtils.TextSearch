using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MbUtils.TextSearch.Domain
{
    public interface IFileInspector
    {
        int GetNumberOfMatches(string filePath, string searchTerm);
    }
}
