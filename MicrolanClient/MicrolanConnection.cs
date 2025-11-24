using System.Net;
using System.Net.Sockets;

namespace MicrolanClient
{
    internal class MicrolanConnection : IDisposable
    {
        public IPAddress IP { get; }
        public int Port { get; }
        private readonly TcpClient _tcpClient;
        private NetworkStream? _stream;
        private bool _disposed = false;

        public MicrolanConnection(string ip, int port = 10900)
        {
            IP = IPAddress.Parse(ip);
            Port = port;
            _tcpClient = new TcpClient();
        }

        public static async Task<MicrolanConnection> CreateAsync(string ip, int port = 10900)
        {
            var connection = new MicrolanConnection(ip, port);
            await connection.ConnectAsync();
            return connection;
        }

        private async Task ConnectAsync()
        {
            if (!_disposed)
            {
                await _tcpClient.ConnectAsync(IP, Port);
                _stream = _tcpClient.GetStream();
            }
            else
            {
                throw new ObjectDisposedException(nameof(MicrolanConnection));
            }
        }

        public static async Task<byte[]> SendCommandAsync(NetworkStream stream, byte[] command)
        {
            var responseBuf = new byte[512];
            await stream.WriteAsync(command);
            int bytesRead = await stream.ReadAsync(responseBuf);
            if (bytesRead == 0)
            {
                throw new IOException("Connection lost: Could not receive response");
            }
            return responseBuf[..bytesRead];
        }

        public async Task<byte[]> OpenDoorAsync()
        {
            if (!_disposed)
            {
                if (_stream == null)
                {
                    throw new InvalidOperationException("Connection not established. Use CreateAsync to create the object");
                }

                byte[] openDoorComand = [0x42, 0x3, 0x1, 0x0, 0x1, 0x17];

                return await SendCommandAsync(_stream, openDoorComand);
            }
            throw new ObjectDisposedException(nameof(MicrolanConnection));
        }

        public async Task<byte[]> ExecuteAsync(byte[] command)
        {
            if (!_disposed)
            {
                if (_stream != null)
                {
                    return await SendCommandAsync(_stream, command);
                }
                else
                {
                    using var tcpClient = new TcpClient();
                    await tcpClient.ConnectAsync(IP, Port);
                    using var netStream = tcpClient.GetStream();
                    return await SendCommandAsync(netStream, command);
                }
            }
            throw new ObjectDisposedException(nameof(MicrolanConnection));
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _tcpClient?.Dispose();
            }

            _disposed = true;
        }

        ~MicrolanConnection()
        {
            Dispose(false);
        }
    }
}