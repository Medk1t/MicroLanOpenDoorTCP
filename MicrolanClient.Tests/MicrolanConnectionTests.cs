using MicrolanClient.Core;
using Moq;
using System.Net;

public class MicrolanConnectionTests
{
    [Fact]
    public async Task ConnectAsync_WhenNotDisposed_ShouldConnectAndGetStream()
    {
        var mockTcpClient = new Mock<ITcpClient>();
        var mockStream = new Mock<IStream>();

        mockTcpClient.Setup(c => c.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>())).Returns(Task.CompletedTask);
        mockTcpClient.Setup(c => c.GetStream()).Returns(mockStream.Object);

        var connection = new MicrolanConnection("192.168.1.100", tcpClient: mockTcpClient.Object);

        await connection.ConnectAsync();

        mockTcpClient.Verify(c => c.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>()), Times.Once);
        mockTcpClient.Verify(c => c.GetStream(), Times.Once);
    }

    [Fact]
    public async Task ConnectAsync_WhenDisposed_ShouldThrowObjectDisposedException()
    {
        var connection = new MicrolanConnection("192.168.1.100");
        connection.Dispose();
        await Assert.ThrowsAsync<ObjectDisposedException>(() => connection.ConnectAsync());
    }

    [Fact]
    public async Task SendCommandAsync_WhenNotDisposedAndConnected_ShouldSendCommandAndReadResponse()
    {
        var mockTcpClient = new Mock<ITcpClient>();
        var mockStream = new Mock<IStream>();

        mockTcpClient.Setup(c => c.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>())).Returns(Task.CompletedTask);
        mockTcpClient.Setup(c => c.GetStream()).Returns(mockStream.Object);
        mockStream.Setup(s => s.WriteAsync(It.IsAny<byte[]>(), 0, It.IsAny<int>())).Returns(Task.CompletedTask);
        mockStream.Setup(s => s.FlushAsync()).Returns(Task.CompletedTask);
        byte[] fakeResponse = new byte[] { 0x01, 0x02, 0x03, 0x04 };
        mockStream.Setup(s => s.ReadAsync(It.IsAny<byte[]>(), 0, 4))
            .Callback<byte[], int, int>((buffer, offset, count) => Array.Copy(fakeResponse, 0, buffer, offset, count))
            .ReturnsAsync(4);

        var connection = new MicrolanConnection("192.168.1.100", tcpClient: mockTcpClient.Object);
        await connection.ConnectAsync();
        byte[] command = new byte[] { 0x42, 0x03 };

        var result = await connection.SendCommandAsync(command);

        mockStream.Verify(s => s.WriteAsync(command, 0, command.Length), Times.Once);
        mockStream.Verify(s => s.FlushAsync(), Times.Once);
        mockStream.Verify(s => s.ReadAsync(It.IsAny<byte[]>(), 0, 4), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(4, result.Length);
        Assert.Equal(fakeResponse, result);
    }

    [Fact]
    public async Task SendCommandAsync_WhenNotConnected_ShouldThrowInvalidOperationException()
    {
        var connection = new MicrolanConnection("192.168.1.100");

        byte[] command = new byte[] { 0x42, 0x03 };
        await Assert.ThrowsAsync<InvalidOperationException>(() => connection.SendCommandAsync(command));
    }

    [Fact]
    public async Task SendCommandAsync_WhenDisposed_ShouldThrowObjectDisposedException()
    {
        var connection = new MicrolanConnection("192.168.1.100");
        connection.Dispose();

        byte[] command = new byte[] { 0x42, 0x03 };

        await Assert.ThrowsAsync<ObjectDisposedException>(() => connection.SendCommandAsync(command));
    }

    [Fact]
    public async Task OpenDoorAsync_WhenNotDisposedAndConnected_ShouldSendCommand()
    {
        var mockTcpClient = new Mock<ITcpClient>();
        var mockStream = new Mock<IStream>();

        mockTcpClient.Setup(c => c.ConnectAsync(It.IsAny<IPAddress>(), It.IsAny<int>())).Returns(Task.CompletedTask);
        mockTcpClient.Setup(c => c.GetStream()).Returns(mockStream.Object);
        mockStream.Setup(s => s.WriteAsync(It.IsAny<byte[]>(), 0, It.IsAny<int>())).Returns(Task.CompletedTask);
        mockStream.Setup(s => s.FlushAsync()).Returns(Task.CompletedTask);
        byte[] fakeResponse = new byte[] { 0x00, 0x00, 0x00, 0x00 };
        mockStream.Setup(s => s.ReadAsync(It.IsAny<byte[]>(), 0, 4))
            .Callback<byte[], int, int>((buffer, offset, count) => Array.Copy(fakeResponse, 0, buffer, offset, count))
            .ReturnsAsync(4);

        var connection = new MicrolanConnection("192.168.1.100", tcpClient: mockTcpClient.Object);
        await connection.ConnectAsync();

        var result = await connection.OpenDoorAsync();

        byte[] expectedCommand = new byte[] { 0x42, 0x03, 0x01, 0x00, 0x01, 0x17 };
        mockStream.Verify(s => s.WriteAsync(expectedCommand, 0, expectedCommand.Length), Times.Once);
        mockStream.Verify(s => s.FlushAsync(), Times.Once);
        mockStream.Verify(s => s.ReadAsync(It.IsAny<byte[]>(), 0, 4), Times.Once);
        Assert.NotNull(result);
        Assert.Equal(4, result.Length);
        Assert.Equal(fakeResponse, result);
    }

    [Fact]
    public async Task OpenDoorAsync_WhenNotConnected_ShouldThrowInvalidOperationException()
    {
        var connection = new MicrolanConnection("192.168.1.100");

        await Assert.ThrowsAsync<InvalidOperationException>(() => connection.OpenDoorAsync());
    }

    [Fact]
    public async Task OpenDoorAsync_WhenDisposed_ShouldThrowObjectDisposedException()
    {
        var connection = new MicrolanConnection("192.168.1.100");
        connection.Dispose();

        await Assert.ThrowsAsync<ObjectDisposedException>(() => connection.OpenDoorAsync());
    }
}
