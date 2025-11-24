using System.Net;

namespace MicrolanClient
{
    public class MicrolanConnection : IDisposable
    {
        public IPAddress IP { get; }
        public int Port { get; }
        private readonly ITcpClient _tcpClient;
        private IStream? _stream;
        private bool _disposed = false;

        public MicrolanConnection(string ip, int port = 10900, ITcpClient? tcpClient = null)
        {
            IP = IPAddress.Parse(ip);
            Port = port;
            _tcpClient = tcpClient ?? new TcpClientAdapter();
        }

        public static async Task<MicrolanConnection> CreateAsync(string ip, int port = 10900, ITcpClient? tcpClient = null)
        {
            var connection = new MicrolanConnection(ip, port, tcpClient);
            await connection.ConnectAsync();
            return connection;
        }

        public async Task ConnectAsync()
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


        public async Task<byte[]> SendCommandAsync(byte[] command)
        {
            if (!_disposed)
            {
                if (_stream == null)
                {
                    throw new InvalidOperationException("Connection not established.");
                }

                // Отправка команды
                await _stream.WriteAsync(command, 0, command.Length);
                await _stream.FlushAsync();  // Используем FlushAsync, если добавили в IStream

                // Чтение ответа (предполагаем 4 байта, как в оригинале)
                byte[] response = new byte[4];
                int bytesRead = await _stream.ReadAsync(response, 0, response.Length);
                if (bytesRead == 0)
                {
                    throw new IOException("Connection lost: Could not receive response");
                }
                return response;
            }
            throw new ObjectDisposedException(nameof(MicrolanConnection));
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

                return await SendCommandAsync(openDoorComand);
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