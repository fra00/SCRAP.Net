using MainRobot;
using MainRobot.Common;
using MainRobot.Robot;
using MainRobot.Robot.Comunication;
using MainRobot.Robot.Comunication.Model;

namespace Robot.Robot.Device.FakeArduino
{
    public class FakeArduino : IFakeArduino
    {
        public FakeArduino(string? fixedResponse = null)
        {
            this.fixedResponse = fixedResponse;
        }
        private string? fixedResponse = null;

        private Action<string> dataReceivedHandler;
        public void OpenSerial(Action<string> dataReceivedHandler)
        {
            this.dataReceivedHandler = dataReceivedHandler;
        }

        private async void calculateResponse(string message)
        {
            var flagNotifica = false;

            var arrayCmd = message.Split(MainRobot.Configuration.SERIAL_SEPARETOR);
            var cmd = new ComunicationCommandReceived(arrayCmd);
            //#ARDU|{cmd.ReceivedId}|{cmd.ReceivedCmd}
            string response = $"{MainRobot.Configuration.SERIAL_START_MESSAGE_ARDU}{MainRobot.Configuration.SERIAL_SEPARETOR}{cmd.ReceivedId}{MainRobot.Configuration.SERIAL_SEPARETOR}{cmd.ReceivedCmd}";
            if (fixedResponse != null)
            {
                response = $"{MainRobot.Configuration.SERIAL_START_MESSAGE_ARDU}{MainRobot.Configuration.SERIAL_SEPARETOR}{cmd.ReceivedId}{MainRobot.Configuration.SERIAL_SEPARETOR}{fixedResponse}";
            }
            else
            {
                //notifica ricezione
                if (cmd.ReceivedCmd == MainRobot.Configuration.SERIAL_END_MESSAGE)
                {
                    flagNotifica = true;
                }
                //disable rele 1
                if (cmd.ReceivedCmd == "05")
                {
                    StatusRobotArdu.Rele1 = false;
                }
                //enable rele 1
                if (cmd.ReceivedCmd == "06")
                {
                    StatusRobotArdu.Rele1 = true;
                }
                //disable rele 2
                if (cmd.ReceivedCmd == "07")
                {
                    StatusRobotArdu.Rele1 = false;
                }
                //enable rele 2
                if (cmd.ReceivedCmd == "08")
                {
                    StatusRobotArdu.Rele1 = true;
                }

                //stop
                if (cmd.ReceivedCmd == "10")
                {
                    //do something
                }

                //forward
                if (cmd.ReceivedCmd == "11")
                {
                    if (cmd.ReceivedParam1 == "555")
                    {
                        //$"12;20;0|90|5"
                        response += $"{MainRobot.Configuration.SERIAL_SEPARETOR}12{MainRobot.Configuration.SERIAL_ARRAY_SEPARETOR}20{MainRobot.Configuration.SERIAL_ARRAY_SEPARETOR}0{MainRobot.Configuration.SERIAL_SEPARETOR}90{MainRobot.Configuration.SERIAL_SEPARETOR}5";
                    }
                }

                //left
                if (cmd.ReceivedCmd == "12")
                {
                    //do something
                }

                //right
                if (cmd.ReceivedCmd == "13")
                {
                    //do something
                }

                //disable obstacle find
                if (cmd.ReceivedCmd == "14")
                {
                    //do something
                }

                //enable moviment
                if (cmd.ReceivedCmd == "21")
                {
                    //do something
                }

                //disable moviment
                if (cmd.ReceivedCmd == "22")
                {
                    //do something
                }

                //disable moviment
                if (cmd.ReceivedCmd == "30")
                {
                    var dist = MathUtil.Distance(StatusRobot.CurrentPosition, RobotConfiguration.PointRecharge);
                    response += $"{MainRobot.Configuration.SERIAL_SEPARETOR}{dist}";
                }

                if (cmd.ReceivedCmd == "thread-sleep")
                {
                    await Task.Delay(2000);
                }

                if (cmd.ReceivedCmd == "thread-sleep1")
                {
                    await Task.Delay(2000);
                }

                if (cmd.ReceivedCmd == "timeout")
                {
                    await Task.Delay(50000);
                }

            }
            //"|00"
            if (!flagNotifica)
                response += MainRobot.Configuration.SERIAL_SEPARETOR + MainRobot.Configuration.SERIAL_END_MESSAGE;
            dataReceivedHandler(response);
        }



        public void WriteLine(string message)
        {
            calculateResponse(message);
        }
    }
}
