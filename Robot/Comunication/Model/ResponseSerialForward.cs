using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Comunication.Model
{
    public class ResponseSerialForward
    {
        public bool Completed { get; set; }
        public int Angle { get; set; }
        public string DistanceObstacle { get; set; }
        public int DistanceRunned { get; set; }
    }
}
