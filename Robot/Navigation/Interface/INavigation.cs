using MainRobot.Common;
using MainRobot.Robot.Navigation.Model;

namespace MainRobot.Robot.Navigation.Interface
{
    public interface INavigation
    {
        /// <summary>
        /// Stop navigation and restart
        /// </summary>
        /// <returns></returns>
        Task ResetNavigation(RPoint? endPoint = null, bool? isForRecharge = false);
        /// <summary>
        /// Navigate to point
        /// </summary>
        /// <param name="end">Point to navigate</param>
        /// <returns></returns>
        Task NavigateTo(RPoint end, bool? isForRecharge = false, bool? continueNavigation = false);
        /// <summary>
        /// Navigate to recharge position
        /// </summary>
        /// <returns></returns>
        Task NavigateToRecharge();
        Task<IEnumerable<(int, float)>?> ReadObstacleFromLidar();
        Task<IEnumerable<(int, float)>?> ReadRawLidar();
        bool[,] GetObstacleInMap();
    }
}