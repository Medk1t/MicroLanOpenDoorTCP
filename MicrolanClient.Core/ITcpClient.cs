using System.Net;

namespace MicrolanClient.Core
{
    public interface ITcpClient : IDisposable
    {
        Task ConnectAsync(IPAddress address, int port);
        IStream GetStream();
    }
}
