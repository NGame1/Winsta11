using System;
//using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryEncoding
{
    public partial class Binary
    {
        public abstract partial class EndianCodec
        {
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            private static T Read<T>(Stream stream, Func<byte[], int, T> func)
            {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));

                if (!stream.CanRead)
                    throw new Exception("Stream is not readable");

#if NETSTANDARD1_1
                var size = Marshal.SizeOf(typeof(T));
#else
                var size = Marshal.SizeOf<T>();
#endif
                var buffer = new byte[size]/*ArrayPool<byte>.Shared.Rent(size)*/;
                T result = default(T);
                try
                { 
                    var bytesRead = stream.Read(buffer, 0, size);
                    if (bytesRead <= 0)
                        throw new EndOfStreamException();

                    if (bytesRead != size)
                        throw new Exception("Could not read full length");

                    result = func(buffer, 0);
                }
                finally
                {
                    buffer = null;
                    //ArrayPool<byte>.Shared.Return(buffer);
                }
                return result;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            private static async Task<T> ReadAsync<T>(Stream stream, Func<byte[], int, T> func, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));

                if (!stream.CanRead)
                    throw new Exception("Stream is not readable");

#if NETSTANDARD1_1
                var size = Marshal.SizeOf(typeof(T));
#else
                var size = Marshal.SizeOf<T>();
#endif
                var buffer = new byte[size]/*ArrayPool<byte>.Shared.Rent(size)*/;
                T result = default(T);
                try
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, size, cancellationToken);
                    if (bytesRead <= 0)
                        throw new EndOfStreamException();
                    if (bytesRead != size)
                        throw new Exception("Could not read full length");

                    result = func(buffer, 0);
                }
                finally
                {
                    buffer = null;
                    //ArrayPool<byte>.Shared.Return(buffer);
                }
                return result;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public short ReadInt16(Stream stream) => Read(stream, GetInt16);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int ReadInt32(Stream stream) => Read(stream, GetInt32);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public long ReadInt64(Stream stream) => Read(stream, GetInt64);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public ushort ReadUInt16(Stream stream) => Read(stream, GetUInt16);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public uint ReadUInt32(Stream stream) => Read(stream, GetUInt32);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public ulong ReadUInt64(Stream stream) => Read(stream, GetUInt64);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<short> ReadInt16Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, GetInt16, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)] 
            public Task<int> ReadInt32Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, GetInt32, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<long> ReadInt64Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, GetInt64, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<ushort> ReadUInt16Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, GetUInt16, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<uint> ReadUInt32Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, GetUInt32, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<ulong> ReadUInt64Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, GetUInt64, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public IEnumerable<short> ReadInt16(Stream stream, int count) => Enumerable.Range(0, count).Select(_ => ReadInt16(stream));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public IEnumerable<int> ReadInt32(Stream stream, int count) => Enumerable.Range(0, count).Select(_ => ReadInt32(stream));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public IEnumerable<long> ReadInt64(Stream stream, int count) => Enumerable.Range(0, count).Select(_ => ReadInt64(stream));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public IEnumerable<ushort> ReadUInt16(Stream stream, int count) => Enumerable.Range(0, count).Select(_ => ReadUInt16(stream));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public IEnumerable<uint> ReadUInt32(Stream stream, int count) => Enumerable.Range(0, count).Select(_ => ReadUInt32(stream));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public IEnumerable<ulong> ReadUInt64(Stream stream, int count) => Enumerable.Range(0, count).Select(_ => ReadUInt64(stream));


            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<short[]> ReadInt16Async(Stream stream, int count, CancellationToken cancellationToken = default(CancellationToken)) => Task.WhenAll(Enumerable.Range(0, count).Select(_ => ReadInt16Async(stream, cancellationToken)));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int[]> ReadInt32Async(Stream stream, int count, CancellationToken cancellationToken = default(CancellationToken)) => Task.WhenAll(Enumerable.Range(0, count).Select(_ => ReadInt32Async(stream, cancellationToken)));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<long[]> ReadInt64Async(Stream stream, int count, CancellationToken cancellationToken = default(CancellationToken)) => Task.WhenAll(Enumerable.Range(0, count).Select(_ => ReadInt64Async(stream, cancellationToken)));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<ushort[]> ReadUInt16Async(Stream stream, int count, CancellationToken cancellationToken = default(CancellationToken)) => Task.WhenAll(Enumerable.Range(0, count).Select(_ => ReadUInt16Async(stream, cancellationToken)));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<uint[]> ReadUInt32Async(Stream stream, int count, CancellationToken cancellationToken = default(CancellationToken)) => Task.WhenAll(Enumerable.Range(0, count).Select(_ => ReadUInt32Async(stream, cancellationToken)));

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<ulong[]> ReadUInt64Async(Stream stream, int count, CancellationToken cancellationToken = default(CancellationToken)) => Task.WhenAll(Enumerable.Range(0, count).Select(_ => ReadUInt64Async(stream, cancellationToken)));


            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            private static int Write<T>(Stream stream, T value, Func<T, byte[], int, int> func)
            {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));

                if (!stream.CanWrite)
                    throw new Exception("Stream is not writable");

#if NETSTANDARD1_1
                var size = Marshal.SizeOf(typeof(T));
#else
                var size = Marshal.SizeOf<T>();
#endif
                var buffer = new byte[size]/*ArrayPool<byte>.Shared.Rent(size)*/;
                int length = -1;
                try
                {
                    length = func(value, buffer, 0);
                    stream.Write(buffer, 0, size);
                }
                finally
                {
                    buffer = null;
                    //ArrayPool<byte>.Shared.Return(buffer);
                }
                return length;
            }


            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            private static async Task<int> WriteAsync<T>(Stream stream, T value, Func<T, byte[], int, int> func, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (stream == null)
                    throw new ArgumentNullException(nameof(stream));

                if (!stream.CanWrite)
                    throw new Exception("Stream is not writable");

#if NETSTANDARD1_1
                var size = Marshal.SizeOf(typeof(T));
#else
                var size = Marshal.SizeOf<T>();
#endif
                var buffer = new byte[size]/*ArrayPool<byte>.Shared.Rent(size)*/;
                int length = -1;
                try
                {
                    length = func(value, buffer, 0);
                    await stream.WriteAsync(buffer, 0, size, cancellationToken);
                }
                finally
                {
                    buffer = null;
                    //ArrayPool<byte>.Shared.Return(buffer);
                }
                return length;
            }


            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, short value) => Write(stream, value, Set);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, int value) => Write(stream, value, Set);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, long value) => Write(stream, value, Set);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, ushort value) => Write(stream, value, Set);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, uint value) => Write(stream, value, Set);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, ulong value) => Write(stream, value, Set);


            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, short value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, value, Set, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, int value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, value, Set, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, long value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, value, Set, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, ushort value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, value, Set, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, uint value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, value, Set, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, ulong value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, value, Set, cancellationToken);


            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, params short[] values) => values.Select(v => Write(stream, v)).Sum();

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, params int[] values) => values.Select(v => Write(stream, v)).Sum();

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, params long[] values) => values.Select(v => Write(stream, v)).Sum();

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, params ushort[] values) => values.Select(v => Write(stream, v)).Sum();

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, params uint[] values) => values.Select(v => Write(stream, v)).Sum();

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public int Write(Stream stream, params ulong[] values) => values.Select(v => Write(stream, v)).Sum();


            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken), params short[] values) => Task.WhenAll(values.Select(v => WriteAsync(stream, v, cancellationToken))).ContinueWith(t => t.Result.Sum());

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken), params int[] values) => Task.WhenAll(values.Select(v => WriteAsync(stream, v, cancellationToken))).ContinueWith(t => t.Result.Sum());

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken), params long[] values) => Task.WhenAll(values.Select(v => WriteAsync(stream, v, cancellationToken))).ContinueWith(t => t.Result.Sum());

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken), params ushort[] values) => Task.WhenAll(values.Select(v => WriteAsync(stream, v, cancellationToken))).ContinueWith(t => t.Result.Sum());

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken), params uint[] values) => Task.WhenAll(values.Select(v => WriteAsync(stream, v, cancellationToken))).ContinueWith(t => t.Result.Sum());

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public Task<int> WriteAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken), params ulong[] values) => Task.WhenAll(values.Select(v => WriteAsync(stream, v, cancellationToken))).ContinueWith(t => t.Result.Sum());
        }
    }
}

