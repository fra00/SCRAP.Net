using Robot.Robot.Device.FakeArduino;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Device
{
    public class FakeSerial : ISerial
    {
        private IFakeArduino fakeArduino;
        public FakeSerial(string? fixedResponse = null, bool? remote = null)
        {
            //ho messo nel costruttore remote a false chiamo fake arduino
            if (remote.HasValue && !remote.Value)
            {
                fakeArduino = new FakeArduino(fixedResponse);
            }
            else
            {
                //se non ho esplicitato che voglio un remote , verifico se ho messo  fake_remote
                if (Configuration.FAKE_REMOTE_ARDUINO)
                    fakeArduino = new FakeRemoteArduino();
                else //altrimenti uso il fake standard
                    fakeArduino = new FakeArduino(fixedResponse);
            }
        }



        public void CloseSerial()
        {
            return;
        }

        public void OpenSerial(Action<string> dataReceivedHandler)
        {
            fakeArduino.OpenSerial(dataReceivedHandler);
        }

        public void OpenSerialByte(string serialName, int speed, Action<byte[]> dataReceivedHandler)
        {
            throw new NotImplementedException();
        }

        public void WriteLine(string message)
        {
            fakeArduino.WriteLine(message);
        }
    }
}
