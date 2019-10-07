namespace FETHArchiveManager
{
    public interface ICompression
    {
        byte[] Compress(byte[] file);
        byte[] Decompress(byte[] file);
    }
}