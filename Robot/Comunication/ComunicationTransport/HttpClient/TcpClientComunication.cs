using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace MainRobot.Robot.Comunication.ComunicationTransport.HttpClient
{
    public class TcpClientComunication : IHttpClientComunication
    {
        private readonly Socket _socket;
        IPEndPoint ipEndPoint;
        private readonly string _host;
        private readonly int _port;

        private const int BufferSize = 1024;

        public TcpClientComunication()
        {
            var splitted = Configuration.TCP_URL_COMUNICATION.Split(";");
            _host = splitted[0];
            _port = int.Parse(splitted[1]);
            
            ipEndPoint = new(IPAddress.Parse(_host), _port);
            _socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }

        public async Task<bool> ConnectAsync()
        {
            if (!_socket.Connected)
            {
                await _socket.ConnectAsync(IPAddress.Parse(_host), _port);

                return true;
            }
            return false;
        }

        public async Task<string> SendAsync(string message)
        {
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                Console.WriteLine($"Tcp start");

                var buffer = new byte[BufferSize];

                if (!_socket.Connected) {
                    await _socket.ConnectAsync(ipEndPoint);
                    //await _socket.SendAsync(Encoding.UTF8.GetBytes(""), SocketFlags.None);
                    //var b = await _socket.ReceiveAsync(buffer);
                    //string hello = Encoding.UTF8.GetString(buffer, 0, b);
                }

                var bytes = Encoding.UTF8.GetBytes(message);
                await _socket.SendAsync(bytes, SocketFlags.None);

                Console.WriteLine($"bytes and SendAsync in  ({stopWatch.ElapsedMilliseconds})");

                int bytesReceived = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                // Process received data based on agreed-upon format
                string response = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                Console.WriteLine($"received string in  ({stopWatch.ElapsedMilliseconds})");
                Console.WriteLine($"Message sent: {message}");
                stopWatch.Stop();
                var r = response.Replace("\r\n", "");
                return r;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}
