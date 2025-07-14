using System.Net;
using System.Net.Sockets;

namespace TCPTest
{
    internal class MicrolanPing
    {
        public IPAddress IP { get; }
        public int Port { get; }
        public MicrolanPing(string IP, int Port = 10900)
        {
            this.IP = IPAddress.Parse(IP);
            this.Port = Port;
        }

        private async Task<NetworkStream> Establish(TcpClient tcpClient)
        {
            await tcpClient.ConnectAsync(IP, Port);
            return tcpClient.GetStream();
        }

        private async Task PushCommand(NetworkStream stream, byte[] command)
        {
            var responseBuf = new byte[512];
            await stream.ReadAsync(responseBuf);
            await stream.WriteAsync(command);
        }

        public async Task Execute(byte[] command)
        {
            var tcpClient = new TcpClient();
            var netStream = await Establish(tcpClient);
            await PushCommand(netStream, command);
            tcpClient.Close();
        }
    }
}
