using DiscountService.CodeService;
using DiscountService.Common.Enumerator;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace DiscountService.TCP
{
    public class TCPServer
    {
        private readonly TcpListener _listener;
        private readonly DiscountCodeService _storage;

        public TCPServer(DiscountCodeService storage)
        {
            _listener = new TcpListener(IPAddress.Any, 5000);
            _storage = storage;
        }

        public async Task Start()
        {
            _listener.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                var client = await _listener.AcceptTcpClientAsync();
                _ = Task.Run(() => HandleClient(client));
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            using var stream = client.GetStream();
            var buffer = new byte[4096];

            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            var requestJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            var request = JsonDocument.Parse(requestJson).RootElement;

            string responseJson = "";
            if (request.GetProperty("Type").GetString() == "Generate")
            {
                ushort count = request.GetProperty("Count").GetUInt16();
                byte length = request.GetProperty("Length").GetByte();
                var codes = await _storage.GenerateAsync(count, length);
                responseJson = JsonSerializer.Serialize(new { Result = true, Codes = codes });
            }
            else if (request.GetProperty("Type").GetString() == "UseCode")
            {
                string? code = request.GetProperty("Code").GetString();
                ResultEnum.Result result = await _storage.UseAsync(code ?? string.Empty);
                responseJson = JsonSerializer.Serialize(new { Result = result });
            }

            var responseBytes = Encoding.UTF8.GetBytes(responseJson);
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        }
    }
} 
