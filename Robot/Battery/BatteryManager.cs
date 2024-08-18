using MainRobot.Robot.Comunication.ComunicationTransport;
using MainRobot.Robot.Comunication.Interface;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.TTS;
using Serilog;

using System.Timers;

namespace MainRobot.Robot.Battery
{
    public class BatteryManager : IBatteryManager
    {
        private ICommandComunication comunication;
        private ITextToSpeach textToSpeach;
        private INavigation navigation;



        private System.Timers.Timer timer;
        public BatteryManager(ICommandComunication comunication,
            INavigation navigation,
            ITextToSpeach textToSpeach)
        {
            this.comunication = comunication;
            this.textToSpeach = textToSpeach;
            this.navigation = navigation;
            //il controllo viene fatto ogni x minuti
            //timer = new System.Timers.Timer(1000 * 60 * 10);
            timer = new System.Timers.Timer(1000 * 60 * 1);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (StatusRobot.IsInRecharge ||
                    StatusRobot.LowBattery ||
                    StatusRobot.Navigating) return;
                var volts = await comunication.GetLevelOfAlimentation();
                if (volts!= null && volts < 9)
                {
                    StatusRobot.LowBattery = true;
                    Log.Logger.Information("Livello batteria basso, tensione :" + volts);
                    textToSpeach.TalkAsync(RobotConfiguration.MESSAGGIO_RICARICA);
                    await this.navigation.NavigateToRecharge();
                }
            }  catch (Exception ex)
            {
                Log.Logger.Information($"BatteryManager error {ex.Message}" );
                Console.WriteLine($"BatteryManager error {ex.Message}");
            }
        }
    }

    public interface IBatteryManager
    {

    }
}

