using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace WinstaCore.Converters.FileConverters
{
    public static class FileConverter
    {
        public static async Task<byte[]> ToBytesAsync(this IRandomAccessStream stream)
        {
            var dr = new DataReader(stream.GetInputStreamAt(0));
            var bytes = new byte[stream.Size];
            await dr.LoadAsync((uint)stream.Size);
            dr.ReadBytes(bytes);
            return bytes;
        }
    }
}
