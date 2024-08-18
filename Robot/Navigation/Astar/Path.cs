using MainRobot.Common;

namespace MainRobot.Robot.Navigation.Astar
{
    public class Path
    {
        public PointPath CurrentPoint;
        public List<PointPath> PathPoint;
        public Path Parent;
        public int Order;
        public double Score;
        public int Tentative_G;


        public Path(PointPath p)
        {
            CurrentPoint = p;
            Order = 0;
            PathPoint = new List<PointPath>();
        }

        public Path(RPoint p)
        {
            CurrentPoint = new PointPath(p.X, p.Y);
            Order = 0;
            PathPoint = new List<PointPath>();
        }


    }

}
