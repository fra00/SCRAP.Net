using MainRobot.Common;
using MainRobot.Robot.Navigation.Model;

namespace MainRobot.Robot.Navigation.Interface
{
    public interface IMovement
    {
        /// <summary>
        /// move backward
        /// </summary>
        /// <param name="distance"></param>
        /// <returns></returns>
        Task<EndMovModel> Backward(int distance);
        //move forward
        Task<EndMovModel> Forward(int distance);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="angle"></param>
        /// <param name="distanceForward"></param>
        /// <param name="backward"></param>
        /// <returns></returns>
        RPoint ObstacleFinded(string distance, int angle, int distanceForward, int backward);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void SetObstacle(int x, int y);
        /// <summary>
        /// Stop 
        /// </summary>
        /// <returns></returns>
        Task Stop();
        /// <summary>
        /// Rotate Robot to left
        /// </summary>
        /// <param name="angleToStop"></param>
        /// <param name="deltaAngle"></param>
        /// <returns></returns>
        Task<EndMovModel> TurnLeft(int angleToStop, int deltaAngle);

        /// <summary>
        /// Rotate Robot to right
        /// </summary>
        /// <param name="angleToStop"></param>
        /// <param name="deltaAngle"></param>
        /// <returns></returns>
        Task<EndMovModel> TurnRight(int angleToStop, int deltaAngle);

        /// <summary>
        /// Rotate robot left or right
        /// </summary>
        /// <param name="left">true for left false for Right</param>
        /// <param name="deltaAngle"></param>
        /// <returns></returns>
        Task<EndMovModel> Rotate(bool left, int deltaAngle);

        Task EnableMoviment();
        Task RotateXCell(int angle);
        Task RotateYCell(int angle);
    }
}