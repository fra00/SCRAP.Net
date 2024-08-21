using MainRobot.Robot.Comunication.WebSocketTransport;
using MainRobot.WebSocketServer;
using System.Diagnostics;

namespace MainRobot.TTS
{
    public class TextToSpeach : ITextToSpeach
    {
        private IWebSocketServer webSocket;

        public TextToSpeach(IWebSocketServer webSocket)
        {
            this.webSocket = webSocket;
        }
        public void TalkAsync(string textToSpeach)
        {
            //if websocket is connected send data to websocket
            if (webSocket.Connected()) {
                this.webSocket.SendAsync(new WebSocketOutputData
                {
                    command = ActionType.Speak,
                    data = textToSpeach
                });
                return;
            }

            // Create a new Process instance
            Process process = new Process();


            // Configure the process to use cmd.exe as the file name
            process.StartInfo.FileName = Configuration.PATH_ESPEAK + "espeak";
            var arguments = Configuration.ARGUMENTS_ESPEAK.Replace("[@textToSpeach]", textToSpeach); 
            // Pass the command to execute as an argument
            process.StartInfo.Arguments = arguments;

            // Redirect the standard output to read it later
            process.StartInfo.RedirectStandardOutput = true;

            // Start the process
            process.Start();

            // Read the output from the process
            //string output = process.StandardOutput.ReadToEnd();

            // Wait for the process to exit
            //process.WaitForExitAsync();

            // Print the output to the console
            //Console.WriteLine(output);
        }

    }
}
