using System;
using System.Collections.Generic;
using System.Text;

namespace MbUtils.TextSearch.Domain
{
    public interface IFilePathProvider
    {
        IEnumerable<string> GetFilePaths(string rootFolderPath);
    }
}
