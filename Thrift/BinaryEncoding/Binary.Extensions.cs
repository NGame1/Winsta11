using System;
using System.Runtime.CompilerServices;

namespace BinaryEncoding
{
    public static partial class Binary
    {
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static byte[] GetBytes(this EndianCodec codec, short value)
        {
            var buffer = new byte[2];
            codec.Set(value, buffer);
            return buffer;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static byte[] GetBytes(this EndianCodec codec, ushort value)
        {
            var buffer = new byte[2];
            codec.Set(value, buffer);
            return buffer;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static byte[] GetBytes(this EndianCodec codec, int value)
        {
            var buffer = new byte[4];
            codec.Set(value, buffer);
            return buffer;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static byte[] GetBytes(this EndianCodec codec, uint value)
        {
            var buffer = new byte[4];
            codec.Set(value, buffer);
            return buffer;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static byte[] GetBytes(this EndianCodec codec, long value)
        {
            var buffer = new byte[8];
            codec.Set(value, buffer);
            return buffer;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public static byte[] GetBytes(this EndianCodec codec, ulong value)
        {
            var buffer = new byte[8];
            codec.Set(value, buffer);
            return buffer;
        }
    }
}

