using MainRobot.Common;

namespace MainRobot.Robot.Navigation.Astar
{
    public interface IAstar {
        Path FindPath(RPoint s, RPoint e);
        void ObstacleEncountered(RPoint point);
        bool[,] GetObstacleInMap();
        /// <summary>
        /// Set obstacle in map but not change weight
        /// </summary>
        /// <param name="point"></param>
        void ObstacleAdd(RPoint point);
        void SetWeightPoints(IEnumerable<RPoint> points, bool increment);
        void SetWeight(RPoint point, bool increment);
        void ClearObstacle();
        bool IsObstacle(RPoint point);
        bool IsWall(RPoint point, int tolerance);
    }
}
