namespace MainRobot.Robot.Device.Lidar
{
    using MainRobot.Common;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Text;
    using System.Linq;


    public class LinesInfo
    {
        public double? Slope { get; set; }
        public double? Intercept { get; set; }
        public double? Angle { get; set; }
        public IEnumerable<RPoint> Points { get; set; }
    }

    public static class LidarUtility
    {
        public static (double pendenza, double intercetta)? CalcolaRetta(RPoint punto1,
                                                                        RPoint punto2)
        {
            if (punto1.X == punto2.X)
            {
                // I punti hanno la stessa x, quindi la retta è verticale e non può essere espressa come funzione.
                return null;
            }
            else
            {
                double m = (punto2.Y - punto1.Y) / (double)(punto2.X - punto1.X); // Calcolo della pendenza (m)
                double q = (double)(punto1.Y - m * punto1.X); // Calcolo dell'intercetta y (q)

                return (m, q); // Restituisce la pendenza e l'intercetta y
            }
        }

        public static IEnumerable<RPoint>? DistanceToPoints(RPoint current, IEnumerable<(int, float)>? distances)
        {
            if (distances == null) return null;
            return distances.Select(c => MathUtil.MovePointOfDistance(current, c.Item2, c.Item1));
        }

        public static LinesInfo? GroupPointsByLine(IEnumerable<RPoint> points)
        {
            var threshold = 2;//5
            var minNunPoints = 6;//5
            List<LinesInfo> lines = new List<LinesInfo>();

            foreach (var pointA in points)
            {
                foreach (var pointB in points)
                {
                    //è lo stesso punto va avanti
                    if (pointA.X == pointB.X && pointA.Y == pointB.Y) continue;

                    if (pointA.X == pointB.X)
                    {
                        //retta verticale
                        var pointsLine = points.Where(c => (c.X > pointA.X - 3) && (c.X < pointA.X + 3)).ToList();

                        if (pointsLine.Count >= minNunPoints)
                        {
                            lines.Add(new LinesInfo
                            {
                                Slope = null,
                                Intercept = null,
                                Angle = 90,
                                Points = pointsLine
                            });
                        }
                    }
                    else
                    {
                        (double pendenza, double intercetta)? eq = CalcolaRetta(pointA, pointB);
                        if (!eq.HasValue) continue;
                        var pointsLine = points.Where(c => c != null)
                                .Where(c => Math.Abs((double)(c.Y - (eq?.pendenza * c.X + eq?.intercetta))) < threshold)
                         ;

                        if (pointsLine.Count() >= minNunPoints)
                        {
                            lines.Add(new LinesInfo
                            {
                                Slope = eq?.pendenza,
                                Intercept = eq?.intercetta,
                                Angle = CalculateLineAngle(eq?.pendenza),
                                Points = pointsLine
                            });
                        }
                    }
                }
            }
            return lines.OrderByDescending(c => c.Points.Count()).FirstOrDefault();
        }

        public static double? CalculateLineAngle(double? slope)
        {
            if (slope == null) return slope;
            const double PI = Math.PI; // Pre-calculate PI for efficiency

            double angleInRadians = Math.Atan(slope.Value);
            double angleInDegrees = angleInRadians * 180 / PI;

            return angleInDegrees;
        }
        public static RPoint? AutoPostiionFromLidar(IEnumerable<(int, float)> distances, Func<RPoint, int,bool> isPointOnWall)
        {
            if (distances == null) return null;

            //transform all distance in points
            IEnumerable<RPoint> points = distances
                .Select(c => MathUtil.MovePointOfDistance(new RPoint(0, 0), c.Item2, (c.Item1 + StatusRobot.CurrentAngle) % 360))
                .Select(c => new RPoint(c.X / RobotConfiguration.MIN_STEP_FOR_FINDPATH * RobotConfiguration.MIN_STEP_FOR_FINDPATH, 
                                        c.Y / RobotConfiguration.MIN_STEP_FOR_FINDPATH * RobotConfiguration.MIN_STEP_FOR_FINDPATH));


            var checkAllMap = true;

            List<RPoint> maxPointOnWall = new List<RPoint>();
            RPoint pointPos = null;

            var minX = points.Min(c => c.X);
            var maxX = points.Max(c => c.X);
            var minY = points.Min(c => c.Y);

            var startX = minX < 0 ? Math.Abs(minX) : 0;
            var startY = minY < 0 ? Math.Abs(minY) : 0;

            var boundX = Math.Abs(minX - maxX);
            var stepX = RobotConfiguration.MIN_STEP_FOR_FINDPATH;
            var stepY = RobotConfiguration.MIN_STEP_FOR_FINDPATH;

            var currentPoint = new RPoint(startX, startY);

            while (checkAllMap)
            {
                List<RPoint> pointOnWall = new List<RPoint>();

                foreach (var pointLidar in points)
                {
                    var traslatedPoint = new RPoint(currentPoint.X + pointLidar.X, currentPoint.Y + pointLidar.Y);
                    if (traslatedPoint.X < 10) break;
                    if (traslatedPoint.Y < 10 || traslatedPoint.X >= 990)
                    {
                        currentPoint = new RPoint(startX, currentPoint.Y + stepY);
                        break;
                    }
                    if (traslatedPoint.Y >= 990)
                    {
                        checkAllMap = false;
                        break;
                    }
                    if (isPointOnWall(new RPoint(traslatedPoint.X, traslatedPoint.Y),12))
                    {
                        pointOnWall.Add(new RPoint(traslatedPoint.X, traslatedPoint.Y));
                    }
                }

                if (pointOnWall.Count > maxPointOnWall.Count)
                {
                    pointPos = new RPoint(currentPoint.X, currentPoint.Y);
                    maxPointOnWall = new List<RPoint>();
                    maxPointOnWall.AddRange(pointOnWall);
                }
                currentPoint = new RPoint(currentPoint.X + stepX, currentPoint.Y);
            }

            //calculate 83% of points
            //double limit = ((float)points.Count() / 100) * 75;
            //if (maxPointOnWall.Count > limit && !isPointOnWall(pointPos, 0))
            if (maxPointOnWall.Count > RobotConfiguration.PERC_POINT_OVERLAP_AUTOPOS_LIDAR 
                && !isPointOnWall(pointPos,0))
                return pointPos;
            return null;
        }
    }
}
