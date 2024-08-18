using MainRobot.Common;
using MainRobot.Robot.Comunication.ComunicationTransport;
using MainRobot.Robot.Comunication.Interface;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Navigation.Recharge.Interface;
using Serilog;

namespace MainRobot.Robot.Navigation.Recharge
{
    public class RechargeManager : IRechargeManager
    {
        private INavigationMover navigationMover;
        private IRechargeNavigation rechargeNavigation;
        private ICommandComunication comunication;


        public RechargeManager(INavigationMover navigationMover,
                               ICommandComunication comunication,
                               IRechargeNavigation rechargeNavigation)
        {
            this.navigationMover = navigationMover;
            this.comunication = comunication;
            this.rechargeNavigation = rechargeNavigation;
        }

        /// <summary>
        /// il robot è arrivato in ricarica setto gli stati in ricarica
        /// </summary>
        /// <returns></returns>
        public async Task PlaceInRecharge()
        {
            //il robot risulta in ricarica setto gli stati per la posizione in ricarica
            //se sono in ricarica , la batteria non può essere low
            StatusRobot.LowBattery = false;
            StatusRobot.IsInRecharge = true;
            StatusRobot.NavigatingToRecharge = false;
            await this.comunication.IsInRecharge();

            //queste voci devono essere messe in un file di configurazione e modificabili a caldo senza 
            //ricompilazione
            RPoint chargePosition = RobotConfiguration.PointRecharge;
            int angleToApproch = RobotConfiguration.AngleStartRecharge;

            navigationMover.UpdatePosition(chargePosition, angleToApproch);

            Log.Logger.Information("Placed in Recharge");
        }

        public async Task ExitFromRecharge()
        {
            await navigationMover.Backward(RobotConfiguration.HALF_WIDTH_ROBOT);
            await this.comunication.EnableObstacleFind();
            await this.comunication.OutInRecharge();
            StatusRobot.IsInRecharge = false;

        }

        /// <summary>
        /// Inizia l'avvicinamento alla base di ricarica 
        /// </summary>
        /// <returns></returns>
        public async Task NavigateToRecharge()
        {
            await rechargeNavigation.StartPositioningInRecharge();
            await this.PlaceInRecharge();
        }


    }
}
