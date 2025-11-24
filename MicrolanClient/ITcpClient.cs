using System.Net;

namespace MicrolanClient
{
    public interface ITcpClient : IDisposable
    {
        Task ConnectAsync(IPAddress address, int port);
        IStream GetStream();
    }
}
