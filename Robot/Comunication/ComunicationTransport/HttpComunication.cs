using MainRobot.Robot.Comunication.ComunicationTransport.HttpClient;
using MainRobot.Robot.Comunication.Model;
using MainRobot.Robot.Device;
using Serilog;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

namespace MainRobot.Robot.Comunication.ComunicationTransport
{
    public class HttpComunication : IComunication
    {

        private IHttpClientComunication httpClientCom;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serial"></param>
        public HttpComunication(IHttpClientComunication httpClientCom)
        {
            this.httpClientCom = httpClientCom;
        }

        public async Task<string> RunCommand(ComunicationCommand currentCommand)
        {
            try
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                Console.WriteLine($"HttpComunication RunCommand exec {currentCommand.Command} {currentCommand.AliasCommand}");
                var content = await this.httpClientCom.SendAsync(currentCommand.Command);
                Log.Logger.Information($"Rpi-http line received ({content})");
                Console.WriteLine("HttpComunication RunCommand result " + content);
                stopWatch.Stop();
                Console.WriteLine($"Command runned in  ({stopWatch.ElapsedMilliseconds})");
                return content;
            }
            catch (Exception ex)
            {
                Log.Logger.Error($"RPI-Error send", ex);
                Console.WriteLine($"HttpComunication RunCommand exception " + ex.Message);
                return null;
            }
        }
    }
}