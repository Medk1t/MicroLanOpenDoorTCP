using System.Net.Sockets;
using System.Net;
using System.Text;
using System.IO;

Console.Title = "Microlan - Ethernet bridge. E2M port emulator.";

var tcpListener = new TcpListener(IPAddress.Any, 10900);
try
{
    tcpListener.Start();    // запускаем сервер
    Console.WriteLine("Сервер запущен. Ожидание подключений... ");
    Console.WriteLine(tcpListener.Server.LocalEndPoint);

    while (true)
    {
        // получаем подключение в виде TcpClient
        var tcpClient = await tcpListener.AcceptTcpClientAsync();

        Console.WriteLine($"Входящее подключение: {tcpClient.Client.RemoteEndPoint}");

        var tcpData = tcpClient.GetStream();
        var message = "Microlan - Ethernet bridge. E2M port.";
        var requestData = Encoding.UTF8.GetBytes(message);

        Console.WriteLine($"Отправляю ответ: {message}");
        await tcpData.WriteAsync(requestData);

        // буфер для получения данных

        var responseData = new byte[512];
        int bytes;  // количество полученных байтов

        do
        {
            // получаем данные
            bytes = await tcpData.ReadAsync(responseData);
            if (bytes > 0)
            {
                var command = new Dictionary<int, string>()
                {
                    { 1, "Open"},
                    { 2, "Close" },
                    { 3, "OpenStay" },
                    { 4, "SetDelayTime" },
                    { 5, "GetStatus" },
                    { 6, "SetNewAddress" },
                    { 7, "GetUPTIME" },
                    { 8, "GetEEPROM" },
                    { 9, "GetDelayTime" },
                    { default, "" }
                };
                var commandName = string.Empty;
                try
                {
                    commandName = command[responseData[4]];
                }
                catch
                {
                    commandName = responseData[5].ToString();
                }
                Console.WriteLine($"LockRS485 \nAddress on RS-485: {responseData[1]} \nCommand: {commandName} \nCommandKey: {responseData[5]}");
                Console.WriteLine($"Debug {responseData[0].ToString("X4")} {responseData[1].ToString("X4")} {responseData[2].ToString("X4")} {responseData[3].ToString("X4")} {responseData[4].ToString("X4")} {responseData[5].ToString("X4")}");
                Console.WriteLine(Encoding.UTF8.GetString(responseData, 4, 1));
                var responseMessage = Encoding.UTF8.GetString(responseData, 0, bytes - 1);
                var responseMessageData = Encoding.UTF8.GetBytes(responseMessage);
                Console.WriteLine($"Отправляю положительный ответ");
                await tcpData.WriteAsync(responseMessageData);
            }
        }
        while (tcpClient.Connected);

    }
}
finally
{
    tcpListener.Stop(); // останавливаем сервер
}
public enum MicrolanCommands : byte
{
    Open = 1,
    Close = 2,
    OpenStay = 3,
    SetDelayTime = 4,
    GetStatus = 5,
    SetNewAddress = 6,
    GetUPTIME = 7,
    GetEEPROM = 8,
    GetDelayTime = 9
}