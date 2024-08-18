using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainRobot.Robot.Comunication.WebSocketTransport;

namespace MainRobot.WebSocketServer
{
    public interface IWebSocketServer
    {
        Task Init();
        Task SendAsync(WebSocketOutputData outputdata);
        Task<string?> SendWithResponseAsync(WebSocketOutputData message);
        Task Echo();
        bool Connected();
        void SetHandler(IWebSocketHandler handler);
    }
}
