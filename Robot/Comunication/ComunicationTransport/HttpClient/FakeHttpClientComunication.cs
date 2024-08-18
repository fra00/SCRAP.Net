using Robot.Common.IO;
using Robot.Common;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace MainRobot.Robot.Comunication.ComunicationTransport.HttpClient
{
    public class FakeHttpClientComunication : IHttpClientComunication
    {
        private FileMonitor<CommandJson> monitorResponse;
        private TaskCompletionSource<string> taskCompletionSource;
        public FakeHttpClientComunication(IHttpClientFactory httpClientFactory)
        {
            monitorResponse = new FileMonitor<CommandJson>("./", "responseCommand.json");
            monitorResponse.FileChanged += (object sender, CommandJson status) =>
            {
                taskCompletionSource.SetResult(status.Command);
                //monitorResponse.Stop();
            };
            monitorResponse.Start();
        }

        public async Task<string> SendAsync(string command)
        {
            taskCompletionSource = new TaskCompletionSource<string>();
            await Task.Delay(500);
            LogFileForEmulator.WriteCommand(command, "command.json");
            var content = await taskCompletionSource.Task;
            taskCompletionSource = null;
            return content;
        }
    }

}
