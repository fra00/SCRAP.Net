using MainRobot.Common;
using MainRobot.Robot.Navigation.Model;

namespace MainRobot.Robot.Navigation.Interface
{
    public interface INavigationMover
    {
        Task<EndMovModel> Backward(int distance);
        Task<EndMovModel> Forward(int distance);
        Task<EndMovModel> Rotate(bool left, int deltaAngle);
        void UpdatePosition(RPoint nextPoint, int angle);
    }
}