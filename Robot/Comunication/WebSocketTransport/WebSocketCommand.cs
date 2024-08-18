using MainRobot.WebSocketServer;

namespace MainRobot.Robot.Comunication.WebSocketTransport
{
    public class WebSocketCommand : IWebSocketCommand
    {
        private IWebSocketServer webSocket;
        public WebSocketCommand(IWebSocketServer webSocket)
        {
            this.webSocket = webSocket;
        }

        public async Task Talk(string text)
        {
            //if websocket is connected send data to websocket
            if (webSocket.Connected())
            {
                await this.webSocket.SendAsync(new WebSocketOutputData
                {
                    command = ActionType.Speak,
                    data = text
                });
                return;
            }
        }

        public async Task<string?> Looks() {
            var r = await webSocket.SendWithResponseAsync(new WebSocketOutputData
            {
                command = ActionType.Photobase64,
            });
            if (r != null)
            {
                var image = r;
                return image;
            }
            return null;
        }
    }
}
