using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace WinstaCore.Converters.FileConverters
{
    public static class FileConverter
    {
        public static async Task<byte[]> ToBytesAsync(this IRandomAccessStream s)
        {
            var dr = new DataReader(s.GetInputStreamAt(0));
            var bytes = new byte[s.Size];
            await dr.LoadAsync((uint)s.Size);
            dr.ReadBytes(bytes);
            return bytes;
        }

        public static async Task<byte[]> ConvertToBytesArray(this IRandomAccessStream stream)
        {
            return await ToBytesAsync(stream);
        }
    }
}
