using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.CustomException
{
    public class ExceptionRunCommand : System.Exception
    {
        public ExceptionRunCommand(string message) : base(message) { }
    }
}
