namespace MainRobot.Robot.Comunication.ComunicationTransport.HttpClient
{
    public interface IHttpClientComunication
    {
        Task<string> SendAsync(string command);
    }
}