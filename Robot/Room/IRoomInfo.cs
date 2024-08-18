using MainRobot.Common;

namespace MainRobot.Robot.Room
{
    public interface IRoomInfo
    {
        RPoint? GetPointRoom(string room);
    }
}