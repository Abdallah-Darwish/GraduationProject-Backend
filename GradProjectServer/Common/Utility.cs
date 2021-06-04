using System;
using System.IO;
using System.Threading.Tasks;

namespace GradProjectServer.Common
{
    public static class Utility
    {
        public static MemoryStream DecodeBase64(string base64)
        {
            MemoryStream res = new();
            res.Write(Convert.FromBase64String(base64));
            res.Flush();
            res.Position = 0;
            return res;
        }

        public static async Task<MemoryStream> DecodeBase64Async(string base64)
        {
            MemoryStream res = new();
            await res.WriteAsync(Convert.FromBase64String(base64)).ConfigureAwait(false);
            await res.FlushAsync().ConfigureAwait(false);
            res.Position = 0;
            return res;
        }
    }
}