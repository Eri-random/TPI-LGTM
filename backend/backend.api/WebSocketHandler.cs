using backend.servicios.DTOs;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace backend.api
{
    public class WebSocketHandler
    {
        private static ConcurrentDictionary<string, WebSocket> webSockets = new ConcurrentDictionary<string, WebSocket>();

        public static async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket, string socketId)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"Recibido: {message}");

                var responseMessage = Encoding.UTF8.GetBytes($"Recibido: {message}");
                await webSocket.SendAsync(new ArraySegment<byte>(responseMessage, 0, responseMessage.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            webSockets.TryRemove(socketId, out _);
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public static async Task BroadcastMessageAsync(string message)
        {
            foreach (var socket in webSockets.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public static async Task NotifyNewDonationAsync(object combinedDonation)
        {
            var message = JsonSerializer.Serialize(new { type = "actualizarDonaciones", data = combinedDonation });
            await BroadcastMessageAsync(message);
        }

        public static void AddSocket(string socketId, WebSocket socket)
        {
            webSockets.TryAdd(socketId, socket);
        }

        public static void RemoveSocket(string socketId)
        {
            webSockets.TryRemove(socketId, out _);
        }
    }
}
