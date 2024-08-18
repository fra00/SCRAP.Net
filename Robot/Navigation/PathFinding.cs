using MainRobot.Common;
using MainRobot.Robot.Navigation.Astar;
using MainRobot.Robot.Navigation.Interface;

namespace MainRobot.Robot.Navigation
{
    public class PathFinding : IPathFinding
    {
        private IAstar pathFinding;

        public PathFinding(IAstar pathFinding)
        {
            this.pathFinding = pathFinding;
        }

        public List<RPoint> FindPath(RPoint start, RPoint end)
        {
            List<RPoint> points = new List<RPoint>();
            Astar.Path path = pathFinding.FindPath(start, end);
            while (path != null)
            {
                points.Add(new RPoint(path.CurrentPoint.X, path.CurrentPoint.Y));
                path = path.Parent;
            }
            points.Reverse();
            return points;
        }
        public void ObstacleEncountered(RPoint point) {
            pathFinding.ObstacleEncountered(point);
        }

        public void SetWeightPoints(IEnumerable<RPoint> points, bool increment)
        {
            pathFinding.SetWeightPoints(points,increment);
        }

        public void SetWeight(RPoint point, bool increment)
        {
            pathFinding.SetWeight(point,increment);
        }

        public void ClearObstacle() {
            pathFinding.ClearObstacle();
        }

        public void ObstacleAdd(RPoint point)
        {
            pathFinding.ObstacleAdd(point);
        }

        public bool IsObstacle(RPoint point)
        {
            return pathFinding.IsObstacle(point);
        }

        public bool[,] GetObstacleInMap()
        {
            return pathFinding.GetObstacleInMap();
        }

        public bool IsWall(RPoint point, int tolerance)
        {
            return pathFinding.IsWall(point,tolerance);
        }
    }
}
