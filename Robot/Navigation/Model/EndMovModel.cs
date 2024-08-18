using MainRobot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Navigation.Model
{
    public class EndMovModel
    {
        public bool Skipped { get; set; }
        public bool Completed { get; set; }
        public bool Recalculate { get; set; }

        public RPoint Point { get; set; }
        public int Angle { get; set; }

    }

    public class DoMovimentEndModel
    {
        public EndMovModel EndMovModel { get; set; }
        public RPoint Point { get; set; }
        public int Angle { get; set; }
        public bool NextMovement { get; set; }
        public int? AngleMoved { get; internal set; }
    }
}
