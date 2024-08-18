using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.NaturalLanguage.Intent
{
    internal class Intent
    {
    }

    /// <summary>
    /// {\"action\":\"move\",\"direction\":\"forward\",\"distance\":INT}\n
    /// {\"action\":\"move\",\"direction\":\"backward\",\"distance\":INT}\n
    /// {\"action\":\"move\",\"direction\":\"left\",\"angle\":INT}\n
    /// {\"action\":\"move\",\"direction\":\"right\",\"angle\":INT}\n
    /// </summary>
    public class IntentMove
    {
        public string direction { get; set; }
        public int distance { get; set; }
    }


    /// <summary>
    /// {\"action\":\"GOTO\",\"destination\":STRING}\n 
    /// </summary>
    public class IntentGoto
    {
        public string destination { get; set; }
    }
    /// <summary>
    /// {\"action\":\"speech\",\"text\":STRING}    
    /// </summary>
    public class IntentSpeech
    {
        public string text { get; set; }
    }
}
