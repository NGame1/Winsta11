namespace Abstractions
{
    public interface WinstaObjectConverter<out T, TT>
    {
        TT SourceObject { get; set; }
        T Convert();
    }
}
