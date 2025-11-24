using System.Net.Sockets;

namespace MicrolanClient
{
    public class NetworkStreamAdapter : IStream
    {
        private readonly NetworkStream _stream;

        public NetworkStreamAdapter(NetworkStream stream)
        {
            _stream = stream;
        }

        public Task WriteAsync(byte[] buffer, int offset, int count) => _stream.WriteAsync(buffer, offset, count);
        public Task<int> ReadAsync(byte[] buffer, int offset, int count) => _stream.ReadAsync(buffer, offset, count);
        public async Task FlushAsync() => await _stream.FlushAsync();
        public void Dispose() => _stream.Dispose();
        public bool CanRead => _stream.CanRead;
        public bool CanWrite => _stream.CanWrite;
    }
}