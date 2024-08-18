using MainRobot.Robot.ActionExec;
using MainRobot.Robot.Comunication.WebSocketTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.WebSocket
{
    public class RobotWebSocketHandler : IWebSocketHandler
    {
        private IActionExec actionExec;
        public RobotWebSocketHandler(IActionExec actionExec)
        {
            this.actionExec = actionExec;
        }
        public WebSocketOutputData? Exec(WebSocketOutputData? dataReceived)
        {
            string sentence = dataReceived.data;

            actionExec.SentenceExec(sentence);
            return dataReceived;

        }
    }
}
