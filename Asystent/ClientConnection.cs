using Fleck;
using System;
using System.Collections.Generic;
using Asystent.procedures;

namespace Asystent {
    public delegate void MessageListener(string message, IWebSocketConnection clientConn);
    public delegate void ServerStartListener();
    
    public class ClientConnection {
        private static string SOCKET_URL = "ws://127.0.0.1:7000";
        private readonly WebSocketServer _wsServer = new WebSocketServer(SOCKET_URL);
        private readonly List<IWebSocketConnection> _connections = new List<IWebSocketConnection>();

        private bool running = false;
        
        public event MessageListener OnMessage;
        public event ServerStartListener OnServerStart;

        public ClientConnection() {
            FleckLog.Level = LogLevel.Error;
            
            _wsServer.RestartAfterListenError = true;
        }

        ~ClientConnection() {
            Console.WriteLine("Closing WebSocket server");
            _wsServer.Dispose();
        }

        public void Connect() {
            if(running) {
                Console.WriteLine("WebSocketServer already running");
                return;
            }
            try {
                _wsServer.Start((socket) => {
                    socket.OnOpen = () => {
                        Console.WriteLine("Client connected");
                        _connections.Add(socket);
                    };
                    socket.OnClose = () => {
                        Console.WriteLine("Client disconnected");
                        _connections.Remove(socket);
                        Playlist.clear();
                        MessageHandler.Clear();
                    };
                    socket.OnMessage = (message) => {
                        OnMessage?.Invoke(message, socket);
                    };
                });

                Console.WriteLine("Server listens for websocket connections at: " + SOCKET_URL);
                running = true;
                OnServerStart?.Invoke();
            }
            catch(Exception e) {
                Console.WriteLine("Cannot start WebSocketServer.\n\tReason: " + e.Message +
                    "\n\tTrying again in 10 seconds");
                
                Utils.setTimeout(() => {
                    Connect();
                }, 10 * 1000);
            }
        }

        public void DistributeMessage(string msg) {
            foreach(var socket in _connections) {
                socket.Send(msg);
            }
        }
    }
}