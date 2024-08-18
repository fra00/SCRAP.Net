using MainRobot.Common;

namespace MainRobot.Robot.Device.Lidar
{
    public interface ILidarManager {
        Task<IEnumerable<(int, float)>?> ReadRawLidar();
        Task<IEnumerable<(int, float)>?> ReadLidar(Action<int, int> callbackPointReceived);
        int? FindAngleFromLidar(RPoint center, IEnumerable<(int, float)> before, IEnumerable<(int, float)> after);
        IEnumerable<(int, float)>? MapRawLidar(IEnumerable<(int, float)>? rawData, Action<int, int> callbackPointReceived);
    }
}
