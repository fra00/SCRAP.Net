using MainRobot.Common;
using MainRobot.Robot.Comunication.Interface;
using Serilog;
using System.Linq;

namespace MainRobot.Robot.Device.Lidar
{
    public class LidarManager : ILidarManager
    {
        private ICommandComunication communication;

        private bool IsSovrapposto(RPoint punto1, RPoint punto2, int tolleranza)
        {
            return Math.Abs(punto1.X - punto2.X) <= tolleranza &&
                   Math.Abs(punto1.Y - punto2.Y) <= tolleranza;
        }

        private IEnumerable<RPoint> TrovaPuntiSovrapposti(IEnumerable<RPoint> before, IEnumerable<RPoint> after, int tolleranza)
        {
            var list = new List<RPoint>();
            foreach (RPoint punto1 in before)
            {
                foreach (RPoint punto2 in after)
                {
                    if (IsSovrapposto(punto1, punto2, tolleranza))
                    {
                        list.Add(new RPoint(punto1.X, punto1.Y));
                    }
                }
            }

            return list;
        }

        public LidarManager(ICommandComunication communication)
        {
            this.communication = communication;
        }

        /// <summary>
        /// L'angolo è assoluto rispetto alla posizione del lidar , non rispetto alla direzione del robot
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<(int, float)>?> ReadRawLidar()
        {
            List<(float, float)> lidarPoints = await this.communication.ReadLidar();
            if (lidarPoints == null)
                return null;

            return lidarPoints.Where(c => 
                    (c.Item2 / 10) > RobotConfiguration.MIN_DISTANCE_LIDAR && c.Item2 / 10 < RobotConfiguration.MAX_DISTANCE_LIDAR
                ).Select(c => (((int)c.Item1+ RobotConfiguration.ANGLE_CORRECTION_LIDAR), c.Item2 / 10)); 
        }


        /// <summary>
        /// il rilevamento dei punti viene fatto in base alla direzione del robot
        /// </summary>
        /// <param name="callbackPointReceived"></param>
        /// <returns></returns>
        public async Task<IEnumerable<(int, float)>?> ReadLidar(Action<int, int> callbackPointReceived)
        {
            IEnumerable<(int, float)>? rawData = await this.ReadRawLidar();
            return MapRawLidar(rawData, callbackPointReceived);
        }

        public IEnumerable<(int, float)>? MapRawLidar(IEnumerable<(int, float)>? rawData, Action<int, int> callbackPointReceived)
        {
            if (rawData != null)
            {
                IEnumerable<RPoint> points = rawData.Select(c =>
                {
                    var normAngle = (StatusRobot.CurrentAngle == 0 ? 360 : StatusRobot.CurrentAngle);
                    int relativeAngle = (normAngle + RobotConfiguration.ORIENTATION_LIDAR + (int)c.Item1) % 360;
                    var r = MathUtil.MovePointOfDistanceRounded10(StatusRobot.CurrentPosition, c.Item2, relativeAngle);
                    return r;
                })
                .Where(c => c.X > 0 && c.Y > 0 && c.X < 1000 && c.Y < 1000);
                foreach (var p in points)
                {
                    if (callbackPointReceived != null)
                    {
                        callbackPointReceived(p.X, p.Y);
                    }
                }
                return rawData;
            }
            return null;
        }

        public int? FindAngleFromLidar(RPoint center, IEnumerable<(int, float)> before, IEnumerable<(int, float)> after)
        {
            int tollerance = 5;
            int minPointsOverlap = 20;

            var incrAngle = 0;

            IEnumerable<RPoint> overlapped = new List<RPoint>();
            IEnumerable<RPoint> transformedPoint = new List<RPoint>();

            var afterCloud = after.Select(c => MathUtil.MovePointOfDistance(center, c.Item2, c.Item1));

            while (true)
            {
                transformedPoint = before.Select(c => MathUtil.MovePointOfDistance(center, c.Item2, c.Item1 + incrAngle));

                overlapped = TrovaPuntiSovrapposti(transformedPoint, afterCloud, tollerance);
                if (overlapped.Count() >= minPointsOverlap)
                {
                    return incrAngle;
                }

                incrAngle += 1;
                if (incrAngle >= 270) return null;
            }
        }
    }
}
