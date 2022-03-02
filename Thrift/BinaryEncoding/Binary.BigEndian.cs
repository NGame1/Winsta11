using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BinaryEncoding
{
    public static partial class Binary
    {
        private class BigEndianCodec : EndianCodec
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(ushort value, List<byte> bytes)
            {
                bytes[0] = (byte)(value >> 8);
                bytes[1] = (byte)value;

                return 2;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(ushort value, byte[] bytes, int offset = 0) => Set(value, bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(short value, List<byte> bytes)
            {
                bytes[0] = (byte)(value >> 8);
                bytes[1] = (byte)value;

                return 2;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(short value, byte[] bytes, int offset = 0) => Set(value, bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(uint value, List<byte> bytes)
            {
                bytes[0] = (byte)(value >> 24);
                bytes[1] = (byte)(value >> 16);
                bytes[2] = (byte)(value >> 8);
                bytes[3] = (byte)value;

                return 4;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(uint value, byte[] bytes, int offset = 0) => Set(value, bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(int value, List<byte> bytes)
            {
                bytes[0] = (byte)(value >> 24);
                bytes[1] = (byte)(value >> 16);
                bytes[2] = (byte)(value >> 8);
                bytes[3] = (byte)value;

                return 4;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(int value, byte[] bytes, int offset = 0) => Set(value, bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(ulong value, List<byte> bytes)
            {
                bytes[0] = (byte)(value >> 56);
                bytes[1] = (byte)(value >> 48);
                bytes[2] = (byte)(value >> 40);
                bytes[3] = (byte)(value >> 32);
                bytes[4] = (byte)(value >> 24);
                bytes[5] = (byte)(value >> 16);
                bytes[6] = (byte)(value >> 8);
                bytes[7] = (byte)value;

                return 8;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(ulong value, byte[] bytes, int offset = 0) => Set(value, bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(long value, List<byte> bytes)
            {
                bytes[0] = (byte)(value >> 56);
                bytes[1] = (byte)(value >> 48);
                bytes[2] = (byte)(value >> 40);
                bytes[3] = (byte)(value >> 32);
                bytes[4] = (byte)(value >> 24);
                bytes[5] = (byte)(value >> 16);
                bytes[6] = (byte)(value >> 8);
                bytes[7] = (byte)value;

                return 8;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int Set(long value, byte[] bytes, int offset = 0) => Set(value, bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override short GetInt16(IReadOnlyList<byte> bytes) => (short)(bytes[1] | bytes[0] << 8);

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override short GetInt16(byte[] bytes, int offset = 0) => GetInt16(bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override ushort GetUInt16(IReadOnlyList<byte> bytes) => (ushort) (bytes[1] | bytes[0] << 8);

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override ushort GetUInt16(byte[] bytes, int offset = 0) => GetUInt16(bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int GetInt32(IReadOnlyList<byte> bytes) =>
                bytes[3] |
                bytes[2] << 8 |
                bytes[1] << 16 |
                bytes[0] << 24;

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override int GetInt32(byte[] bytes, int offset = 0) => GetInt32(bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override uint GetUInt32(IReadOnlyList<byte> bytes) =>
                (uint)bytes[3] |
                (uint)bytes[2] << 8 |
                (uint)bytes[1] << 16 |
                (uint)bytes[0] << 24;

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override uint GetUInt32(byte[] bytes, int offset = 0) => GetUInt32(bytes.Skip(offset).ToArray());

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override long GetInt64(IReadOnlyList<byte> bytes) =>
                (long)bytes[7] |
                (long)bytes[6] << 8 |
                (long)bytes[5] << 16 |
                (long)bytes[4] << 24 |
                (long)bytes[3] << 32 |
                (long)bytes[2] << 40 |
                (long)bytes[1] << 48 |
                (long)bytes[0] << 56;

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override long GetInt64(byte[] bytes, int offset = 0) => GetInt64(bytes.Skip(offset).ToArray());

            public override ulong GetUInt64(IReadOnlyList<byte> bytes) =>
                (ulong)bytes[7] |
                (ulong)bytes[6] << 8 |
                (ulong)bytes[5] << 16 |
                (ulong)bytes[4] << 24 |
                (ulong)bytes[3] << 32 |
                (ulong)bytes[2] << 40 |
                (ulong)bytes[1] << 48 |
                (ulong)bytes[0] << 56;

            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.NoOptimization)]
            public override ulong GetUInt64(byte[] bytes, int offset = 0) => GetUInt64(bytes.Skip(offset).ToArray());
        }
    }
}

