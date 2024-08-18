namespace MainRobot.Robot.Navigation.Recharge.Interface
{
    public interface IRechargeManager
    {
        Task NavigateToRecharge();
        Task ExitFromRecharge();
        Task PlaceInRecharge();
    }
}
