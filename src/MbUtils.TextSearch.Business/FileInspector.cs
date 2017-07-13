using MbUtils.TextSearch.Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MbUtils.TextSearch.Business
{
    public class FileInspector : IFileInspector
    {
        readonly int bufferSize;
        readonly Encoding encoding;

        public FileInspector(int bufferSize, bool isUtf8)
        {
            this.bufferSize = bufferSize;
            encoding = isUtf8 ? Encoding.UTF8 : Encoding.ASCII;
        }

        public async Task<int> GetNumberOfMatchesAsync(string filePath, string searchTerm)
        {
            var ret = 0;
            var buffer = new byte[bufferSize];

            using (var fs = File.OpenRead(filePath))
            {
                var readBytesCount = bufferSize;
                while(readBytesCount == bufferSize)
                {
                    readBytesCount = await fs.ReadAsync(buffer, 0, bufferSize);
                    var asString = encoding.GetString(buffer, 0, readBytesCount);
                }
               
            }

            return ret;
        }

        private async Task<IEnumerable<string>> GetChunkAsStringAsync(Stream inputStream)
        {
            throw new NotImplementedException();
        }
    }
}
