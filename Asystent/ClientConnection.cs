using Fleck;
using System;
using System.Collections.Generic;

namespace Asystent {
    public delegate void MessageListener(string message, IWebSocketConnection clientConn);
    
    public class ClientConnection {
        private static string SOCKET_URL = "ws://127.0.0.1:7000";
        private readonly WebSocketServer _wsServer = new WebSocketServer(SOCKET_URL);
        private readonly List<IWebSocketConnection> _connections = new List<IWebSocketConnection>();
        
        public event MessageListener OnMessage;

        public ClientConnection() {
            FleckLog.Level = LogLevel.Error;
            
            Run();
        }

        ~ClientConnection() {
            Console.WriteLine("Closing WebSocket server");
            _wsServer.Dispose();
        }

        private void Run()
        {
            _wsServer.RestartAfterListenError = true;
            _wsServer.Start((socket) =>
            {
                socket.OnOpen = () =>
                {
                    Console.WriteLine("Client connected");
                    _connections.Add(socket);
                };
                socket.OnClose = () =>
                {
                    Console.WriteLine("Client disconnected");
                    _connections.Remove(socket);
                };
                socket.OnMessage = (message) => {
                    OnMessage?.Invoke(message, socket);
                    //socket.Send(message);
                };
            });
            
            Console.WriteLine("Server listens for websocket connections at: " + SOCKET_URL);
        }

        public void DistributeMessage(string msg) {
            foreach(var socket in _connections) {
                socket.Send(msg);
            }
        }
    }
}