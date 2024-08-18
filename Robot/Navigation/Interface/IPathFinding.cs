using MainRobot.Common;

namespace MainRobot.Robot.Navigation.Interface
{
    public interface IPathFinding
    {
        /// <summary>
        /// Calculate the path from a starting point to a 
        /// destination using an A* algorithm
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        List<RPoint> FindPath(RPoint start, RPoint end);
        /// <summary>
        /// Set obstacle at point specified
        /// </summary>
        /// <param name="point"></param>
        void ObstacleEncountered(RPoint point);
        /// <summary>
        /// Set obstacle at point specified  without change weight
        /// </summary>
        /// <param name="point"></param>
        void ObstacleAdd(RPoint point);
        /// <summary>
        /// Set weight of list points
        /// </summary>
        /// <param name="points"></param>
        /// <param name="increment"></param>
        void SetWeightPoints(IEnumerable<RPoint> points, bool increment);
        /// <summary>
        /// Increment weight of a point
        /// </summary>
        /// <param name="point"></param>
        /// <param name="increment"></param>
        void SetWeight(RPoint point, bool increment);
        bool IsObstacle(RPoint point);
        /// <summary>
        /// clear all obstacle
        /// </summary>
        void ClearObstacle();

        /// <summary>
        /// Return all obstacle in map
        /// </summary>
        /// <returns></returns>
        bool[,] GetObstacleInMap();

        bool IsWall(RPoint point, int tolerance);
    }
}
