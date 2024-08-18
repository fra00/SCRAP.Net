using MainRobot.Robot.Comunication.Model;

namespace MainRobot.Robot.Comunication.ComunicationTransport
{
    public interface IComunication
    {
        Task<string> RunCommand(ComunicationCommand? currentCommand);
    }
}