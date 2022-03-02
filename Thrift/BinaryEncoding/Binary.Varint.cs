using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
#pragma warning disable CS0675 // Bitwise-or operator used on a sign-extended operand

namespace BinaryEncoding
{
    public partial class Binary
    {
        public static class Varint
        {
            public const int MaxVarintLen16 = 3;
            public const int MaxVarintLen32 = 5;
            public const int MaxVarintLen64 = 10;

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int GetByteCount(short value) => GetByteCount((ulong)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int GetByteCount(int value) => GetByteCount((ulong)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int GetByteCount(long value) => GetByteCount((ulong)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int GetByteCount(ushort value) => GetByteCount((ulong)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int GetByteCount(uint value) => GetByteCount((ulong)value);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int GetByteCount(ulong value)
            {
                int i;
                for (i = 0; i < 9 && value >= 0x80; i++, value >>= 7) { }
                return i + 1;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static byte[] GetBytes(short value) => GetBytes((ulong) value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static byte[] GetBytes(int value) => GetBytes((ulong) value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static byte[] GetBytes(long value) => GetBytes((ulong) value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static byte[] GetBytes(ushort value) => GetBytes((ulong) value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static byte[] GetBytes(uint value) => GetBytes((ulong) value);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static byte[] GetBytes(ulong value)
            {
                var buffer = new byte[GetByteCount(value)];
                Write(buffer, 0, value);
                return buffer;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(List<byte> buffer, ushort value) => Write(buffer, (ulong)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(byte[] buffer, int offset, ushort value) => Write(buffer.Skip(offset).ToList(), (ulong)value);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(List<byte> buffer, short value) => Write(buffer, (long)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(byte[] buffer, int offset, short value) => Write(buffer.Skip(offset).ToList(), (long)value);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(byte[] buffer, int offset, uint value) => Write(buffer.Skip(offset).ToList(), (ulong)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(List<byte> buffer, uint value) => Write(buffer, (ulong)value);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(byte[] buffer, int offset, int value) => Write(buffer.Skip(offset).ToList(), (long)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(List<byte> buffer, int value) => Write(buffer, (long)value);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(byte[] buffer, int offset, ulong value) => Write(buffer.Skip(offset).ToList(), value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(List<byte> buffer, ulong value)
            {
                int i = 0;
                while (value >= 0x80)
                {
                    buffer[i] = (byte)(value | 0x80);
                    value >>= 7;
                    i++;
                }
                buffer[i] = (byte)value;
                return i + 1;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(byte[] buffer, int offset, long value) => Write(buffer.Skip(offset).ToList(), value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(List<byte> buffer, long value)
            {
                var ux = (ulong)value << 1;
                if (value < 0)
                    ux ^= ux;

                return Write(buffer, ux);
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(Stream stream, ushort value) => Write(stream, (ulong) value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(Stream stream, uint value) => Write(stream, (ulong)value);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(Stream stream, ulong value)
            {
                int i = 0;
                while (value >= 0x80)
                {
                    stream.WriteByte((byte)(value | 0x80));
                    value >>= 7;
                    i++;
                }
                stream.WriteByte((byte) value);
                return i + 1;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<int> WriteAsync(Stream stream, ushort value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, (ulong)value, cancellationToken);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<int> WriteAsync(Stream stream, uint value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, (ulong)value, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static async Task<int> WriteAsync(Stream stream, ulong value, CancellationToken cancellationToken = default(CancellationToken))
            {
                int i = 0;
                byte[] buffer = new byte[1];
                while (value >= 0x80)
                {
                    buffer[0] = (byte)(value | 0x80);
                    await stream.WriteAsync(buffer, 0, 1, cancellationToken);
                    value >>= 7;
                    i++;
                }
                buffer[0] = (byte)value;
                await stream.WriteAsync(buffer, 0, 1, cancellationToken);
                return i + 1;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(Stream stream, short value) => Write(stream, (long)value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(Stream stream, int value) => Write(stream, (long)value);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Write(Stream stream, long value)
            {
                var ux = (ulong)value << 1;
                if (value < 0)
                    ux ^= ux;

                return Write(stream, ux);
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<int> WriteAsync(Stream stream, short value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, (long)value, cancellationToken);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<int> WriteAsync(Stream stream, int value, CancellationToken cancellationToken = default(CancellationToken)) => WriteAsync(stream, (long)value, cancellationToken);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<int> WriteAsync(Stream stream, long value, CancellationToken cancellationToken = default(CancellationToken))
            {
                var ux = (ulong)value << 1;
                if (value < 0)
                    ux ^= ux;

                return WriteAsync(stream, ux, cancellationToken);
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(byte[] buffer, int offset, out ushort value) => Read(buffer.Skip(offset).ToArray(), out value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(IReadOnlyList<byte> buffer, out ushort value)
            {
                var n = Read(buffer, out ulong l);
                value = (ushort)l;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(byte[] buffer, int offset, out short value) => Read(buffer.Skip(offset).ToArray(), out value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(IReadOnlyList<byte> buffer, out short value)
            {
                var n = Read(buffer, out long l);
                value = (short)l;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(byte[] buffer, int offset, out uint value) => Read(buffer.Skip(offset).ToArray(), out value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(IReadOnlyList<byte> buffer, out uint value)
            {
                var n = Read(buffer, out ulong l);
                value = (uint)l;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(byte[] buffer, int offset, out int value) => Read(buffer.Skip(offset).ToArray(), out value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(IReadOnlyList<byte> buffer, out int value)
            {
                var n = Read(buffer, out long l);
                value = (int)l;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(byte[] buffer, int offset, out ulong value) => Read(buffer.Skip(offset).ToArray(), out value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(IReadOnlyList<byte> buffer, out ulong value)
            {
                value = 0;
                int s = 0;
                for (var i = 0; i < buffer.Count; i++)
                {
                    if (buffer[i] < 0x80)
                    {
                        if (i > 9 || i == 9 && buffer[i] > 1)
                        {
                            value = 0;
                            return -(i + 1); // overflow
                        }
                        value |= (ulong)(buffer[i] << s);
                        return i + 1;
                    }
                    value |= (ulong)(buffer[i] & 0x7f) << s;
                    s += 7;
                }
                value = 0;
                return 0;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(byte[] buffer, int offset, out long value) => Read(buffer.Skip(offset).ToArray(), out value);
            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(IReadOnlyList<byte> buffer, out long value)
            {
                int n = Read(buffer, out ulong ux);
                value = (long)(ux >> 1);
                if ((ux & 1) != 0)
                    value ^= value;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(Stream stream, out ushort value)
            {
                var n = Read(stream, out ulong l);
                value = (ushort)l;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(Stream stream, out uint value)
            {
                var n = Read(stream, out ulong l);
                value = (uint)l;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(Stream stream, out ulong value)
            {
                value = 0;
                int s = 0;
                for (var i = 0; ; i++)
                {
                    var r = stream.ReadByte();
                    if (r == -1)
                        throw new EndOfStreamException();

                    var b = (byte)r;
                    if (b < 0x80)
                    {
                        if (i > 9 || i == 9 && b > 1)
                        {
                            return i + 1;
                        }
                        value |= (ulong)(b << s);
                        return i + 1;
                    }
                    value |= (ulong)(b & 0x7f) << s;
                    s += 7;
                }
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(Stream stream, out short value)
            {
                var n = Read(stream, out ulong l);
                value = (short)l;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(Stream stream, out int value)
            {
                var n = Read(stream, out ulong l);
                value = (int)l;
                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static int Read(Stream stream, out long value)
            {
                var n = Read(stream, out ulong ux);
                value = (long)(ux >> 1);
                if ((ux & 1) != 0)
                    value ^= value;

                return n;
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static async Task<ulong> ReadAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
            {
                ulong value = 0;
                int s = 0;
                byte[] buffer = new byte[1];
                for (var i = 0; ; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (await stream.ReadAsync(buffer, 0, 1, cancellationToken) < 1)
                        throw new EndOfStreamException();

                    if (buffer[0] < 0x80)
                    {
                        if (i > 9 || i == 9 && buffer[0] > 1)
                        {
                            return value;
                        }
                        value |= (ulong)(buffer[0] << s);
                        return value;
                    }
                    value |= (ulong)(buffer[0] & 0x7f) << s;
                    s += 7;
                }
            }

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<short> ReadInt16Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, cancellationToken)
                .ContinueWith(t => (short)t.Result, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<int> ReadInt32Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, cancellationToken)
                .ContinueWith(t => (int)t.Result, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<long> ReadInt64Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, cancellationToken)
                .ContinueWith(t =>
                {
                    var value = (long)(t.Result >> 1);
                    if ((t.Result & 1) != 0)
                        value ^= value;
                    return value;
                }, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<ushort> ReadUInt16Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, cancellationToken)
                .ContinueWith(t => (ushort)t.Result, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<uint> ReadUInt32Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, cancellationToken)
                .ContinueWith(t => (uint)t.Result, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.NotOnFaulted);

            [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
            public static Task<ulong> ReadUInt64Async(Stream stream, CancellationToken cancellationToken = default(CancellationToken)) => ReadAsync(stream, cancellationToken);
        }
    }
}

