namespace MicrolanClient.Core
{
    public interface IStream : IDisposable
    {
        Task WriteAsync(byte[] buffer, int offset, int count);
        Task<int> ReadAsync(byte[] buffer, int offset, int count);
        Task FlushAsync();
        bool CanRead { get; }
        bool CanWrite { get; }
    }
}
