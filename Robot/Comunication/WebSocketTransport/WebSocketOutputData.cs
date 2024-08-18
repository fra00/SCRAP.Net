using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Comunication.WebSocketTransport
{
    public class WebSocketOutputData
    {
        public string id { get; set; }
        public string command { get; set; }
        public string data { get; set; }

    }
}
