using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Asystent.procedures;
using vtortola.WebSockets;

namespace Asystent {
    public delegate void MessageListener(string message, WebSocket clientConn);
    public delegate void ServerStartListener();
    
    public class ClientConnection {
        //private static string SOCKET_URL = "ws://127.0.0.1:7000";
        private readonly WebSocketListener _server = new WebSocketListener(new IPEndPoint(IPAddress.Any, 8006));
        private readonly List<WebSocket> _connections = new List<WebSocket>();
        private readonly CancellationTokenSource _cancellation = new CancellationTokenSource();
        private bool _running;
        private Task _task;
        
        public event MessageListener OnMessage;
        public event ServerStartListener OnServerStart;

        private ClientConnection() {
            var rfc6455 = new WebSocketFactoryRfc6455();
            _server.Standards.RegisterStandard(rfc6455);
        }

        private static ClientConnection _instance;
		public static ClientConnection Instance() {
            return _instance ??= new ClientConnection();
        }

        ~ClientConnection() {
            Console.WriteLine("Closing WebSocket server");
            
            _cancellation.Cancel();
            _task?.Wait();
        }

        public void Connect() {
            if(_running) {
                Console.WriteLine("WebSocketServer already running");
                return;
            }
            try {
                _server.StartAsync();
                _task = Task.Run(() => AcceptWebSocketClientsAsync(_server, _cancellation.Token));

                _running = true;
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
        
        private async Task AcceptWebSocketClientsAsync(WebSocketListener server, CancellationToken token) {
            while (!token.IsCancellationRequested)
            {
                try {
                    var ws = await server.AcceptWebSocketAsync(token).ConfigureAwait(false);
                    if (ws != null)
                        await Task.Run(() => HandleConnectionAsync(ws, token));
                }
                catch(Exception e) {
                    Console.WriteLine("Error Accepting clients: " + e.GetBaseException().Message);
                }
            }
        }
        
        private async Task HandleConnectionAsync(WebSocket ws, CancellationToken cancellation) {
            try {
                Console.WriteLine("Client connected");
                _connections.Add(ws);
                Playlist.updatePlaylistsList();
                
                while (ws.IsConnected && !cancellation.IsCancellationRequested) {
                    var message = await ws.ReadStringAsync(CancellationToken.None)
                        .ConfigureAwait(false);
                    if (message != null)
                        OnMessage?.Invoke(message, ws);
                }
                
                Console.WriteLine("Client disconnected");
                _connections.Remove(ws);
                Playlist.clear();
                MessageHandler.Clear();
            }
            catch (Exception aex) {
                Console.WriteLine("Error Handling connection: " + aex.GetBaseException().Message);
                try {
                    ws.Close();
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }
            finally {
                ws.Dispose();
            }
        }

        public void DistributeMessage(string msg) {
            foreach(var socket in _connections) {
                socket.WriteString(msg);
            }
        }
    }
}