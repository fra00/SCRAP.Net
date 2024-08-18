using MainRobot.Robot.Battery;
using MainRobot.TTS;
using MainRobot.Robot.Comunication.ComunicationTransport;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Navigation.Recharge.Interface;
using MainRobot.Robot.Comunication.Interface;

namespace MainRobot.Robot
{
    /// <summary>
    /// Main class robot
    /// </summary>
    public class Robot : IRobot
    {
        private INavigation navigation { get; set; }
        private IRechargeManager rechargeMaanger{ get; set; }
        private ITextToSpeach textToSpeach { get; set; }
        private IBatteryManager batteryManager { get; set; }
        private ICommandComunication comunication { get; set; }
        public Robot(INavigation navigation,
                    IRechargeManager rechargeMaanger,
                    ITextToSpeach textToSpeach,
                    IBatteryManager batteryManager,
                    ICommandComunication comunication)
        {
            

            this.navigation = navigation;
            this.rechargeMaanger= rechargeMaanger;
            this.textToSpeach = textToSpeach;
            this.batteryManager = batteryManager;
            this.comunication = comunication;

        }

        /// <summary>
        /// Set initial status of robot
        /// </summary>
        /// <returns></returns>
        public async Task InitRobot() {
            Thread.Sleep(10000);
            textToSpeach.TalkAsync(RobotConfiguration.MESSAGGIO_AVVIO);
            StatusRobot.InitStatus();
            //force and clean dovrebbe resettare e reimpostare arduino , se è stato bloccato o c'è qualcosa che non 
            //va su arduino . da verificare se serve
            await comunication.ForceArduCleanStatus();
            //all'avvio lo stato iniziale dei relè è disabilitato
            await comunication.DisableRele1();
            await comunication.DisableRele2();
            //abilita la gestione degli ostacoli
            await comunication.EnableObstacleFind();
            //verifico se sono in base di ricarica
            var sourceAlim = await comunication.GetSourceOfAlimentation();
            if (sourceAlim == 1)
            {
                //se l'alimentazione è della rete vuol dire che sono in ricarica aggiorno 
                //aggiorno lo stato per la ricarica
                await rechargeMaanger.PlaceInRecharge();

            }

            textToSpeach.TalkAsync(RobotConfiguration.MESSAGGIO_AVVIO_COMPLETATO);
        }
    }
}