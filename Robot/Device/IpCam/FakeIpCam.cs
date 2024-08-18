using MainRobot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Device.IpCam
{
    /// <summary>
    /// Class fake ipcam for emulator
    /// </summary>
    public class FakeIpCam : IIpCam
    {

        public FakeIpCam()
        {

        }

        public Task<Image<Rgba32>> GetSnapshot()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsDark()
        {
            return await Task.Run(() =>
            {
                return false;
            });
        }

        public async Task<RPoint> TryFindPointOfColor(Rgba32 color, Rgba32 tollerance, short? maxAttempts = 10)
        {
            var currPoint = StatusRobot.CurrentPosition;

            var basePoint = RobotConfiguration.PointRecharge;

            var pointLeft = MathUtil.MovePointOfDistance(basePoint, 10, 90);
            var pointCenter = basePoint;
            var pointRight = MathUtil.MovePointOfDistance(basePoint, 10, -90);

            //trovo l'angolo che c'è tra il robot e la base

            var angleTollerance = 15;

            if (color == RobotConfiguration.SxLedColorRecharge)
            {
                var angleL = MathUtil.AngleBetweenTwoPoints(currPoint, pointLeft);
                var deltaL = MathUtil.DifferenceTwoAngleZero(StatusRobot.CurrentAngle, Math.Abs(angleL));
                if (Math.Abs(deltaL) < angleTollerance) return new RPoint(30, 120);
            }
            if (color == RobotConfiguration.FrontLedColorRecharge)
            {
                //var angleC = MathUtil.AngleBetweenTwoPoints(currPoint, pointCenter);
                var angleC1 = MathUtil.AngleBetweenTwoPoint180Origin(pointCenter, currPoint);
                var deltaC = MathUtil.DifferenceTwoAngleZero(StatusRobot.CurrentAngle, Math.Abs(angleC1));
                if (deltaC > 45) return null;
                if (deltaC < 5) return new RPoint(160, 120);
                else
                {
                    if ((StatusRobot.CurrentAngle - Math.Abs(angleC1)) < 0) return new RPoint(240, 120);
                    else return new RPoint(100, 120);
                    //if (angleC1 > 0) return new RPoint(100, 120);
                    //else return new RPoint(240, 120); 
                }
            }
            if (color == RobotConfiguration.DxLedColorRecharge)
            {
                var angleR = MathUtil.AngleBetweenTwoPoints(currPoint, pointRight);
                var deltaR = MathUtil.DifferenceTwoAngleZero(StatusRobot.CurrentAngle, Math.Abs(angleR));
                if (Math.Abs(deltaR) < angleTollerance) return new RPoint(270, 120);
            }

            return null;
        }
    }
}
