using MainRobot.Robot.Comunication.WebSocketTransport;
using Newtonsoft.Json;
using System.Net;
using System.Net.WebSockets;
using System.ServiceModel.Channels;
using System.Text;

namespace MainRobot.WebSocketServer
{
    public class WebSocketServer : IWebSocketServer
    {
        private WebSocket? webSocket;
        private Dictionary<string, TaskCompletionSource<string>> pendingRequests = new Dictionary<string, TaskCompletionSource<string>>();
        private StringBuilder messageCompose = new StringBuilder();
        private IWebSocketHandler handler;
        public WebSocketServer()
        {
            Init();
        }


        private async Task HandleClientAsync()
        {
            if (webSocket == null)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            var buffer = new byte[1024 * 4];
            var messageCompose = new StringBuilder();
            WebSocketReceiveResult result;
            var closeSent = false;
            var eom = false;
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        
                        if (msg.IndexOf("[EOM]") >= 0)
                        {
                            msg = msg.Replace("[EOM]", "");
                            eom = true;
                        }

                        messageCompose.Append(msg);
                        //if (result.EndOfMessage)
                        if (eom)
                        {
                            string fullMessage = messageCompose.ToString();
                            Console.WriteLine($"Received message from client: {fullMessage}");

                            try
                            {
                                WebSocketOutputData? receivedData = JsonConvert.DeserializeObject<WebSocketOutputData>(fullMessage);
                                if (receivedData == null)
                                {
                                    Console.WriteLine($"Received message from client NULL");
                                    return;
                                }
                                if (pendingRequests.TryGetValue(receivedData.id, out var tcs))
                                {
                                    // Questa è una risposta a una nostra richiesta
                                    tcs.SetResult(receivedData.data);
                                    pendingRequests.Remove(receivedData.id);
                                }
                                else
                                {
                                    // Questa è una nuova richiesta dal client
                                    // Gestisci qui la richiesta del client
                                    handler.Exec(receivedData);
                                }

                                //Dopo aver processato il messaggio, iniziamo la chiusura se non è già stata avviata
                                //if (!closeSent)
                                //{
                                //    await webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Closing from client", CancellationToken.None);
                                //    closeSent = true;
                                //}
                            }
                            catch (JsonException ex)
                            {
                                Console.WriteLine($"Error deserializing message: {ex.Message}");
                            }
                            finally
                            {
                                eom = false;
                                messageCompose.Clear();
                            }
                            
                        }
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing from client", CancellationToken.None);
                        break;
                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            //finally
            //{
            //    Console.WriteLine("Client disconnected");
            //    if (webSocket != null)
            //    {
            //        webSocket.Dispose();
            //    }
            //}
        }

        public async Task Init()
        {
            var listener = new HttpListener();
            //listener.Prefixes.Add("http://localhost:5001/ws/");
            listener.Prefixes.Add("http://*:5001/ws/");
            listener.Start();
            Console.WriteLine("Listening...");
            try
            {
                while (true)
                {
                    var context = await listener.GetContextAsync();
                    if (context.Request.IsWebSocketRequest)
                    {
                        var wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                        webSocket = wsContext.WebSocket;
                        //await Echo();
                        await HandleClientAsync();
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                        context.Response.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                // Handle error gracefully (e.g., log the error)
            }
            finally
            {
                if (listener != null)
                {
                    listener.Stop();
                    listener.Close();
                }

                if (webSocket != null)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Server shutting down", CancellationToken.None);
                }
                //if there is an error close and restart server for websocket
                Init();
            }
        }

        public bool Connected()
        {
            return webSocket != null;
        }

        public async Task SendAsync(WebSocketOutputData message)
        {

            if (webSocket == null || webSocket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("WebSocket is not connected or not open");
            }
            message.id = string.IsNullOrEmpty(message.id) ? Guid.NewGuid().ToString() : message.id;
            byte[] messageBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

        }

        public async Task<string?> SendWithResponseAsync(WebSocketOutputData message)
        {
            message.id = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<string>();
            pendingRequests[message.id] = tcs;

            await SendAsync(message);
            string r = await tcs.Task;

            return r;
        }


        public async Task Echo()
        {
            if (webSocket is null)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public void SetHandler(IWebSocketHandler handler)
        {
            this.handler = handler;
        }
    }



}

