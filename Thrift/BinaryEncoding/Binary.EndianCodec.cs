using System;
using System.Collections.Generic;

namespace BinaryEncoding
{
    public static partial class Binary
    {
        public abstract partial class EndianCodec
        {
            public abstract short GetInt16(byte[] bytes, int offset = 0);
            public abstract ushort GetUInt16(byte[] bytes, int offset = 0);
            public abstract int GetInt32(byte[] bytes, int offset = 0);
            public abstract uint GetUInt32(byte[] bytes, int offset = 0);
            public abstract long GetInt64(byte[] bytes, int offset = 0);
            public abstract ulong GetUInt64(byte[] bytes, int offset = 0);

            public abstract short GetInt16(IReadOnlyList<byte> bytes);
            public abstract ushort GetUInt16(IReadOnlyList<byte> bytes);
            public abstract int GetInt32(IReadOnlyList<byte> bytes);
            public abstract uint GetUInt32(IReadOnlyList<byte> bytes);
            public abstract long GetInt64(IReadOnlyList<byte> bytes);
            public abstract ulong GetUInt64(IReadOnlyList<byte> bytes);

            public abstract int Set(short value, byte[] bytes, int offset = 0);
            public abstract int Set(ushort value, byte[] bytes, int offset = 0);
            public abstract int Set(int value, byte[] bytes, int offset = 0);
            public abstract int Set(uint value, byte[] bytes, int offset = 0);
            public abstract int Set(long value, byte[] bytes, int offset = 0);
            public abstract int Set(ulong value, byte[] bytes, int offset = 0);

            public abstract int Set(short value, List<byte> bytes);
            public abstract int Set(ushort value, List<byte> bytes);
            public abstract int Set(int value, List<byte> bytes);
            public abstract int Set(uint value, List<byte> bytes);
            public abstract int Set(long value, List<byte> bytes);
            public abstract int Set(ulong value, List<byte> bytes);
        }
    }
}

