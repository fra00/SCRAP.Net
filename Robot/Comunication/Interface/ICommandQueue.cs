using MainRobot.Robot.Comunication.Model;

namespace MainRobot.Robot.Comunication.Interface
{
    public interface ICommandQueue
    {
        Task<ComunicationCommandReceived> Enqueue(string command, string logDescriptionCmd);
    }
}