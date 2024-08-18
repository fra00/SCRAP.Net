using MainRobot.Common;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Navigation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainRobot.Robot.Navigation
{
    public class NavigationMover : INavigationMover
    {

        private IMovement movement;

        public NavigationMover(IMovement movement)
        {
            this.movement = movement;
        }

        /// <summary>
        /// update current position and angle
        /// </summary>
        /// <param name="nextPoint"></param>
        /// <param name="angle"></param>
        public void UpdatePosition(RPoint nextPoint, int angle)
        {
            StatusRobot.CurrentPosition = nextPoint;
            StatusRobot.CurrentAngle = angle;
            StatusRobot.UpdateStatus();
        }

        /// <summary>
        /// rotate 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="deltaAngle"></param>
        /// <returns></returns>
        public async Task<EndMovModel> Rotate(bool left, int deltaAngle)
        {
            //negativo gira a sx positivo a dx
            var nextAngle = left ? MathUtil.DifferenceTwoAngle(StatusRobot.CurrentAngle, deltaAngle) :
                MathUtil.SumTwoAngle(StatusRobot.CurrentAngle, deltaAngle);
            EndMovModel r = await this.movement.Rotate(left, deltaAngle);
            this.UpdatePosition(StatusRobot.CurrentPosition, (int)nextAngle);
            return r;
        }

        /// <summary>
        /// move forward
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        public async Task<EndMovModel> Forward(int distance)
        {
            EndMovModel r = await this.movement.Forward(distance);
            var newPos = MathUtil.MovePointOfDistance(StatusRobot.CurrentPosition, distance, StatusRobot.CurrentAngle);
            this.UpdatePosition(newPos, StatusRobot.CurrentAngle);
            return r;
        }

        public async Task<EndMovModel> Backward(int distance)
        {
            EndMovModel r = await this.movement.Backward(distance);
            var newPos = MathUtil.MovePointOfDistance(StatusRobot.CurrentPosition, -distance, StatusRobot.CurrentAngle);
            this.UpdatePosition(newPos, StatusRobot.CurrentAngle);
            return r;
        }

    }


}
