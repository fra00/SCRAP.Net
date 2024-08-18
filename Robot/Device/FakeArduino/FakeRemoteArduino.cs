using MainRobot.Robot;
using Robot.Common;
using Robot.Common.IO;

namespace Robot.Robot.Device.FakeArduino
{
    public class FakeRemoteArduino : IFakeArduino
    {
        private FileMonitor<CommandJson> monitorResponse ;
        public FakeRemoteArduino()
        {
            monitorResponse = new FileMonitor<CommandJson>("./", "responseCommand.json");
            monitorResponse.FileChanged += (object sender, CommandJson status) =>
            {
                calculateResponse(status.Command);
                //monitorResponse.Stop();
            };
            monitorResponse.Start();
        }
        
        private Action<string> dataReceivedHandler;
        public void OpenSerial(Action<string> dataReceivedHandler)
        {
            this.dataReceivedHandler = dataReceivedHandler;
        }

        private async void calculateResponse(string response)
        {
            dataReceivedHandler(response);
           
        }

        public async void WriteLine(string message)
        {
            //monitorResponse.Start();
            await Task.Delay(500);
            LogFileForEmulator.WriteCommand(message,"command.json");
        }
    }

    
}
