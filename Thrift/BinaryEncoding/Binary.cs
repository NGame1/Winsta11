namespace BinaryEncoding
{
    public static partial class Binary
    {
        public static readonly EndianCodec BigEndian = new BigEndianCodec();
        public static readonly EndianCodec LittleEndian = new LittleEndianCodec();
    }
}
