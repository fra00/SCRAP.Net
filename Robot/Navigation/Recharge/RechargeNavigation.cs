using MainRobot.Common;
using MainRobot.Robot.Comunication.ComunicationTransport;
using MainRobot.Robot.Comunication.Interface;
using MainRobot.Robot.Device.IpCam;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Navigation.Recharge.Interface;
using MainRobot.TTS;
using Serilog;

namespace MainRobot.Robot.Navigation.Recharge
{
    public class RechargeNavigation : IRechargeNavigation
    {
        private INavigationMover navigationMover;
        private IMovement movement;
        private ICommandComunication comunication;
        private ITextToSpeach textToSpeach;
        private IIpCam ipCam;

        public RechargeNavigation(INavigationMover navigationMover,
                               IMovement movement,
                               ICommandComunication comunication,
                               ITextToSpeach textToSpeach,
                               IIpCam ipcam)
        {
            this.navigationMover = navigationMover;
            this.movement = movement;
            this.comunication = comunication;
            this.textToSpeach = textToSpeach;
            this.ipCam = ipcam;
        }

        /// <summary>
        /// Esegue i movimenti di avvicinamento fino alla base di ricarica
        /// </summary>
        /// <returns></returns>
        public async Task StartPositioningInRecharge()
        {
            //deve attivare l'ip cam
            await comunication.EnableRele2();

            //è un fake hardware aggiungo un errore per fare in modo che 
            //si sposti di un valore random per simulare gli errori di 
            //posizionamento
            if (MainRobot.Configuration.FAKE_HW)
            {
                var random = new Random();

                var xRandom = new Random().Next(-20, 20);
                var yRandom = new Random().Next(-20, 20);
                var aRandom = new Random().Next(-20, 20);


                navigationMover.UpdatePosition(new RPoint(StatusRobot.CurrentPosition.X + xRandom,
                                                StatusRobot.CurrentPosition.Y + yRandom),
                                                StatusRobot.CurrentAngle + aRandom);
            }

            Log.Logger.Information("start positioning for recharge");

            //se arriva alla ricarica con una posizione diversa da quella 
            //di ricarica gira fino ad arrivare alla giusta angolazione
            var isCorrectAngleRecharge = await RotateToAngleStartRecharge();
            if (!isCorrectAngleRecharge) return;

            //sono al corretto angolo inizio l'avvicinamento alla base di ricarica
            Log.Logger.Information("Start approach to recharge");
            try
            {
                await this.ApproachToRecharge();
            }
            catch (Exception ex)
            {
                if (ex.Message == "baseNotFound")
                {
                    textToSpeach.TalkAsync("Non riesco a trovare la base di ricarica");
                }
                if (ex.Message == "baseCantReach")
                {
                    textToSpeach.TalkAsync("Non riesco raggiungere la base");
                }
                return;
            }
            Log.Logger.Information("approach terminated");
            
        }
        /// <summary>
        /// search a point of central led of base recharge
        /// </summary>
        /// <returns></returns>
        private async Task<RPoint> findColorCentralLed()
        {
            return await this.ipCam.TryFindPointOfColor(RobotConfiguration.FrontLedColorRecharge,
                                                        RobotConfiguration.FrontLedTolleranceRecharge);
        }
        /// <summary>
        /// search a point of left led of base recharge
        /// </summary>
        /// <returns></returns>
        private async Task<RPoint> findColorLeftLed()
        {
            return await this.ipCam.TryFindPointOfColor(RobotConfiguration.SxLedColorRecharge,
                                                        RobotConfiguration.SxLedTolleranceRecharge);
        }
        /// <summary>
        /// search a point of right led of base recharge
        /// </summary>
        /// <returns></returns>
        private async Task<RPoint> findColorRightLed()
        {
            return await this.ipCam.TryFindPointOfColor(RobotConfiguration.DxLedColorRecharge,
                                                        RobotConfiguration.DxLedTolleranceRecharge);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RotateToAngleStartRecharge()
        {
            //ruoto fino a che non arrivo all'angolo di avvicinamento alla baase di ricarica
            while (MathUtil.DifferenceTwoAngleZero(StatusRobot.CurrentAngle, RobotConfiguration.AngleStartRecharge) >= 90)
            {
                var r = await navigationMover.Rotate(true, 90);
                if (r == null || !r.Completed)
                {
                    textToSpeach.TalkAsync("Non riesco a mettermi in posizione per la ricarica");
                    return false;
                }
            }
            return true;
        }
        public async Task<RPoint> rotateToFindCentralLed(bool left)
        {
            RPoint point = null;
            short attemptTurn = 0;
            point = await findColorCentralLed();
            while (attemptTurn < 5 && point == null)
            {
                var r = await navigationMover.Rotate(left, 9);
                if (r.Completed)
                {
                    point = await findColorCentralLed();
                    attemptTurn += 1;
                }
            }
            return point;
        }
        public async Task centerOnBaseChargeDirection(bool small)
        {
            int angleMove = small ? 2 : 3;
            int tolleranceX = 5;
            int centerImage = 320 / 2; //widthImage ipcam /2

            RPoint pCentralLed = await findColorCentralLed();
            if (pCentralLed == null) throw new Exception("can't find central led");
            while (Math.Abs(pCentralLed.X - centerImage) > tolleranceX)
            {
                if ((pCentralLed.X - centerImage) < 0)
                    await navigationMover.Rotate(true, angleMove);
                else
                    await navigationMover.Rotate(false, angleMove);
                pCentralLed = await findColorCentralLed();
            }
            Log.Logger.Information("Base centrata");
        }
        public async Task centerOnBaseLateralLed()
        {
            bool direction = false;
            while (true)
            {
                RPoint ledSx = null;
                RPoint ledDx = null;
                if (!StatusRobot.FakeMoviment)
                {
                    await centerOnBaseChargeDirection(false);
                    ledSx = await findColorLeftLed();
                    ledDx = await findColorRightLed();
                }
                else
                {
                    ledSx = new RPoint(0, 0);
                    ledDx = new RPoint(0, 0);
                }

                bool dirLeft = false;
                //sono stati trovati entrambi i led della base di ricarica, sono centrato
                if (ledSx != null && ledDx != null) break;
                //non sono centrato ai tre led cerca di allinearsi a tre
                if (ledSx != null) dirLeft = false;
                if (ledDx != null) dirLeft = true;

                await navigationMover.Rotate(dirLeft, 90);
                await navigationMover.Forward(3);
                await navigationMover.Rotate(!dirLeft, 90);
            }
            Log.Logger.Information("sono allineato");
        }
        public async Task startApproach()
        {

            bool isFar = true;
            while (isFar)
            {
                var distance = await this.comunication.GetDistanceFrontSensor();
                //se ritorna 0 o è troppo distante o non ha fatto una lettura valida deve riprovare
                //if (distance == 0) continue;
                if (distance <= 30) isFar = false;
                await navigationMover.Forward(8);
                if (!StatusRobot.FakeMoviment)
                {
                    await centerOnBaseChargeDirection(true);
                }
            }
            bool isNear = true;
            int numTentative = 0;
            await this.comunication.DisableObstacleFind();
            while (isNear)
            {
                if (numTentative > 20)
                {
                    throw new Exception("baseCantReach");
                }
                var nearDist = await this.comunication.GetDistanceFrontSensor();
                if (nearDist > 15)
                {
                    await navigationMover.Forward(2);
                }
                else
                {
                    isNear = false;
                }
                numTentative += 1;
            }
            //ultimo step
            await navigationMover.Forward(10);
            Log.Logger.Information("Avvicinamento terminato");
        }

        /// <summary>
        /// start approach to recharge
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task ApproachToRecharge()
        {

            RPoint ledFront = null;

            //se è un fake Moviment ignoro la procedura
            if (!StatusRobot.FakeMoviment)
            {
                //verifico la luminosità dell'immagine
                var isDark = this.ipCam.IsDark();
                //cerco il led che mi indica la base (led centrale)
                ledFront = await findColorCentralLed();
                if (ledFront == null)
                {
                    //se non trovo la base ruoto per trovare il led a sx
                    ledFront = await rotateToFindCentralLed(true);
                    //se non la trovo a sx poi provo a dx
                    if (ledFront == null)
                    {
                        //non ho trovato il led girando di 45 gradi a sx 
                        //ritorno alla posiozne di partenza
                        await navigationMover.Rotate(false, 45);
                        ledFront = await rotateToFindCentralLed(false);
                    }
                }
            }
            else
            {
                //è un fakeMoviment simulo la posizione della base al centro
                ledFront = new RPoint(320, 240);
            }
            if (ledFront == null)
            {
                throw new Exception("baseNotFound");
            }
            Log.Logger.Information($"base trovata point:{ledFront.X},{ledFront.Y}");
            //sono centrato verifico e mi allineo con gli altri due led
            await centerOnBaseLateralLed();
            //sono allineato mi avvicino
            await startApproach();

            await this.comunication.EnableObstacleFind();
            await this.comunication.DisableMoviment();

        }
    }
}

