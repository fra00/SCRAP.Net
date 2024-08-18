namespace MainRobot.Robot.Comunication.WebSocketTransport
{
    public interface IWebSocketCommand
    {
        Task Talk(string text);
        Task<string?> Looks();
    }
}