using System.Net;
using System.Net.Sockets;

namespace MicrolanClient.Core
{
    public class TcpClientAdapter : ITcpClient
    {
        private readonly TcpClient _tcpClient = new TcpClient();
        public async Task ConnectAsync(IPAddress address, int port) => await _tcpClient.ConnectAsync(address, port);
        public NetworkStream GetStream() => _tcpClient.GetStream();
        IStream ITcpClient.GetStream() => new NetworkStreamAdapter(_tcpClient.GetStream());
        public void Dispose()
        {
            _tcpClient?.Dispose();
        }
    }
}
