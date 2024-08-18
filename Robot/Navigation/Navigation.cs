using MainRobot.Common;
using Newtonsoft.Json;
using MainRobot.TTS;
using Serilog;
using MainRobot.Robot.Device.IpCam;
using MainRobot.Robot.Navigation.Model;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Navigation.Recharge.Interface;
using MainRobot.Robot.Comunication.Interface;
using MainRobot.Robot.CustomException;
using MainRobot.Robot.Device.Lidar;

namespace MainRobot.Robot.Navigation
{
    /// <summary>
    /// Class to find path and navigate
    /// </summary>
    public class Navigation : INavigation
    {
        private INavigationMover navigationMover;
        private IMovement movement;
        private ICommandComunication comunication;
        private IRechargeManager rechargeManager;
        private ILidarManager lidarManager;
        private IPathFinding pathFinding;
        private ITextToSpeach textToSpeach;
        private IIpCam ipCam;

        private List<RPoint> pointsPath = new List<RPoint>();
        private List<RPoint> forwardPoint = new List<RPoint>();
        private RPoint startForwardPoint;
        private RPoint endForwardPoint;
        private RPoint endPosition = null;


        private int sumForward = 0;
        private bool stopNavigation = false;
        private int maxTentativeNavigation = 3;
        private int countTentativeNavigation = 0;

        private bool doAutoposition = true;

        private List<RPoint> listLidarPointNavigation = new List<RPoint>();

        private int getFinalAngle(RPoint nextPoint, RPoint oldPoint)
        {
            //calculate angle from 2 point
            int angle = (int)MathUtil.AngleBetweenTwoPoints(oldPoint, nextPoint);

            return angle + (angle < 0 ? 360 : 0);
        }

        /// <summary>
        /// make moviment
        /// </summary>
        /// <param name="next"></param>
        /// <returns></returns>
        private async Task<DoMovimentEndModel> DoMoviment(RPoint next)
        {
            RPoint nextPoint = next;
            int nextAngle = getFinalAngle(nextPoint, StatusRobot.CurrentPosition);
            this.forwardPoint = new List<RPoint>();

            //calculate all points where robot go to in same directione for same segment
            while (getFinalAngle(nextPoint, StatusRobot.CurrentPosition) == StatusRobot.CurrentAngle)
            {
                if (this.sumForward == 0)
                {
                    this.startForwardPoint = nextPoint;
                }
                if (pathFinding.IsObstacle(nextPoint))
                {
                    //finded obstacle on the path recalculate navigation
                    pathFinding.ClearObstacle();
                    return new DoMovimentEndModel
                    {
                        NextMovement = false,
                        EndMovModel = new EndMovModel { Recalculate = true },
                        Angle = StatusRobot.CurrentAngle,
                        Point = StatusRobot.CurrentPosition
                    };
                }
                sumForward += 10;
                this.forwardPoint.Add(nextPoint);
                endForwardPoint = this.pointsPath.First();
                this.pointsPath.Remove(endForwardPoint);
                if (!this.pointsPath.Any()) break;
                if (sumForward > RobotConfiguration.MAX_DISTANCE_FORWARD_CONSEC) break;
                nextPoint = this.pointsPath.First();
            }

            //se non devo andare diritto e ho una somma di volte in cui devo andare avanti
            //vuol dire che devo eseguire il movimento avanti
            if (sumForward > 0)
            {
                Log.Logger.Information("DoMoviment Forward for " + sumForward);
                EndMovModel fwdResult = await movement.Forward(sumForward);
                return new DoMovimentEndModel
                {
                    EndMovModel = fwdResult,
                    Angle = StatusRobot.CurrentAngle,
                    //se ritorna Point vuol dire che ha una posizione diversa da quella che era stata calcolata
                    Point = fwdResult.Point == null ? endForwardPoint : fwdResult.Point,
                    NextMovement = true
                };
            }

            // Calcola la differenza tra l'angolo corrente e quello da raggiungere
            int angleToMove = nextAngle - StatusRobot.CurrentAngle;
            // Normalizza la differenza tra -180 e 180 gradi
            angleToMove = (angleToMove + 180) % 360 - 180;
            if (angleToMove == -270) angleToMove = 90;
            if (angleToMove == 270) angleToMove = -90;

            // Controlla se l'angolo da raggiungere è a sinistra o a destra
            bool turnLeft = angleToMove < 0;

            Log.Logger.Information((turnLeft ? "DoMoviment TurnLeft for " : "DoMoviment TurnRight for ") + angleToMove);
            EndMovModel resTurn = turnLeft ? await movement.TurnLeft(nextAngle, Math.Abs(angleToMove)) :
                                     await movement.TurnRight(nextAngle, Math.Abs(angleToMove));
            return new DoMovimentEndModel
            {
                EndMovModel = resTurn,
                Angle = nextAngle,
                Point = StatusRobot.CurrentPosition,
                AngleMoved = angleToMove
            };

            //return null;
        }

        /// <summary>
        /// action to terminate moviment
        /// </summary>
        /// <param name="endModel"></param>
        /// <returns></returns>
        private async Task MovimentTerminated(DoMovimentEndModel endModel)
        {
            //se ho completato il movimento aggiorno e pulisco i dati per l'avanzamento 
            if (endModel.EndMovModel.Completed)
            {
                //in caso di ostacolo il metodo viene interrotto prima qui ho
                //solo il caso in cui non ci sono stati ostacoli
                //se sumForward � valorizzato vuol dire che ho eseguito una serie di moviementi avanti 
                if (sumForward > 0)
                {
                    StatusRobot.CurrentPosition = endForwardPoint;
                    this.endForwardPoint = null;
                    this.startForwardPoint = null;
                    this.sumForward = 0;

                    Log.Logger.Information("......START Set weight for path point count " + forwardPoint.Count());
                    //se sono arrivato alla fine del ciclo vuol dire che non ho incontrato ostacoli
                    // diminuisco il peso di ogni punto passato
                    //considero la larghezza del robot
                    var width = (RobotConfiguration.HALF_WIDTH_ROBOT * 2) -10;
                    List<RPoint> pointToClear = new List<RPoint>();
                    foreach (var p in forwardPoint)
                    {
                        var mx = p.X - width;
                        var mxx = p.X + width;
                        var my = p.Y - width;
                        var myy = p.Y + width;

                        for (var i = my; i < myy; i = i + 10)
                            for (var j = mx; j < mxx; j = j + 10)
                            {
                                pointToClear.Add(new RPoint(j, i));
                            }
                    }
                    pathFinding.SetWeightPoints(pointToClear, false);
                    forwardPoint = null;
                }
            }
            //sia che ho finito o no il movimento aggiorno la posizione
            navigationMover.UpdatePosition(endModel.Point, endModel.Angle);
        }

        /// <summary>
        /// Action to terminate navigation
        /// </summary>
        /// <returns></returns>
        private async Task NavigationTerminated(bool? pathNotFound = null)
        {
            StatusRobot.Navigating = false;
            countTentativeNavigation = 0;

            //TODO messageForEndNavigation

            pathFinding.ClearObstacle();
            await comunication.DisableRele1();
            await comunication.DisableRele2();
            await comunication.DisableMoviment();


            if (!pathNotFound.HasValue)
            {
                //if moviment are terminated setWeight of point
                var grouped = listLidarPointNavigation.GroupBy(c => new { c.X, c.Y });
                var stimatedPoints = grouped.Where(c => c.Count() > 3).Select(c =>
                {
                    var p = c.First();
                    return new RPoint(p.X, p.Y);
                });
                if (stimatedPoints.Count() > 0)
                {
                    pathFinding.SetWeightPoints(stimatedPoints, true);
                }
            }

        }

        private void mapDistanceFromLidar(IEnumerable<(int, float)>? distances, bool? trackPoints = null)
        {
            if (distances == null) return;
            IEnumerable<(int, float)>? dataLidar = lidarManager.MapRawLidar(distances, (int x, int y) =>
            {
                //movement.SetObstacle(x, y);
                var p = new RPoint(x / 10 * 10, y / 10 * 10);
                if (trackPoints.HasValue && trackPoints.Value)
                    listLidarPointNavigation.Add(p);
                pathFinding.ObstacleAdd(p);
            });
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="movement">Class for moviment</param>
        /// <param name="comunication">Class for comunication</param>
        /// <param name="pathFinding">Class for pathfinding</param>
        /// <param name="textToSpeach">class for speach</param>
        /// <param name="ipCam">class for ipcam</param>
        public Navigation(INavigationMover navigationMover,
                            IMovement movement,
                            ICommandComunication comunication,
                            IRechargeManager rechargeManager,
                            ILidarManager lidarManager,
                            IPathFinding pathFinding,
                            ITextToSpeach textToSpeach,
                            IIpCam ipCam)
        {
            this.navigationMover = navigationMover;
            this.movement = movement;
            this.comunication = comunication;
            this.rechargeManager = rechargeManager;
            this.lidarManager = lidarManager;
            this.pathFinding = pathFinding;
            this.textToSpeach = textToSpeach;
            this.ipCam = ipCam;
        }



        /// <summary>
        /// Stop current navigation and restart new navigation
        /// </summary>
        /// <param name="endPoint">Facoltative if is null destination is endPosition</param>
        /// <returns></returns>
        public async Task ResetNavigation(RPoint? endPoint = null, bool? isForRecharge = false)
        {
            //if someone calls reset Navigation but charging has already started, exit
            if (!isForRecharge.Value && StatusRobot.NavigatingToRecharge) return;

            var ePoint = endPoint ?? this.endPosition;
            if (StatusRobot.Navigating)
            {
                stopNavigation = true;

                while (StatusRobot.Navigating)
                {
                    //aspetto che venga terminato il movimento in corso prima di
                    //iniziare la nuova navigazione
                }
                StatusRobot.Navigating = false;
                stopNavigation = false;
            }

            this.sumForward = 0;

            await this.NavigateTo(ePoint, isForRecharge,true);
        }



        /// <summary>
        /// search path and navigate to point
        /// </summary>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task NavigateTo(RPoint end, bool? isForRecharge = false,bool? continueNavigation=false)
        {
            try
            {
                // In this case robot try to approcah to recharge
                if (isForRecharge.HasValue && !isForRecharge.Value && StatusRobot.NavigatingToRecharge) return;

                if (doAutoposition)
                {
                    var autoPosDist = (await ReadRawLidar())?.ToList();
                    //if under 25 probabily i can't have 80 point with other data read from lidar
                    if (autoPosDist.Count > 28)
                    {
                        //TODO: why i made 2 read of lidar???
                        var other = (await ReadRawLidar())?.ToList();
                        if (other != null)
                            autoPosDist.AddRange(other);
                        other = (await ReadRawLidar())?.ToList();
                        if (other != null)
                            autoPosDist.AddRange(other);
                        if (autoPosDist.Count > 90)
                        {
                            var stimatePOint = LidarUtility.AutoPostiionFromLidar(autoPosDist, (RPoint p, int toll) =>
                            {
                                return pathFinding.IsWall(p, toll);
                            });
                            if (stimatePOint != null)
                            {
                                var dx = Math.Abs(StatusRobot.CurrentPosition.X - stimatePOint.X);
                                var dy = Math.Abs(StatusRobot.CurrentPosition.Y - stimatePOint.Y);
                                if (dx > RobotConfiguration.MAX_DISTANCE_AUTOPOS_LIDAR || dy > RobotConfiguration.MAX_DISTANCE_AUTOPOS_LIDAR)
                                {
                                    //la distanza è troppo distante dall'ultima posizione conosciuta , bisogna fare
                                    //dei movimenti supplementari per verificare se la posizione è corretta o se è
                                    //un errore di posizionamento
                                    //TODO
                                }
                                else
                                {
                                    navigationMover.UpdatePosition(stimatePOint, StatusRobot.CurrentAngle);
                                }

                            }
                            //doAutoposition = false;
                        }
                    }
                }

                //before start moviment read lidar data
                IEnumerable<(int, float)>? lidarBefore = await ReadRawLidar();
                mapDistanceFromLidar(lidarBefore);

                if (StatusRobot.Navigating)
                {
                    //start new navigation
                    this.ResetNavigation(end);
                    return;
                }

                StatusRobot.Navigating = true;

                //preparo l'hardware per la navigazione
                if (continueNavigation == null || continueNavigation.Value == false)
                {
                    await comunication.EnableRele1();
                    await comunication.EnableRele2();
                    await comunication.EnableMoviment();
                }

                if (StatusRobot.IsInRecharge)
                {
                    await rechargeManager.ExitFromRecharge();
                }

                this.sumForward = 0;
                this.endPosition = end;

                // cerco il path
                this.pointsPath = this.pathFinding.FindPath(StatusRobot.CurrentPosition, end);
                if (RobotConfiguration.LOG_FILE_EMULATOR)
                {
                    LogFileForEmulator.Write(JsonConvert.SerializeObject(this.pointsPath), "path.json");
                }

                // se non esiste nessun path disponibile lo comunico tramite la voce ed esco
                if (!this.pointsPath.Any())
                {
                    if (countTentativeNavigation == maxTentativeNavigation)
                    {
                        textToSpeach.TalkAsync("Non ho trovato nessun percorso per raggiungere il punto che mi hai detto");
                        await NavigationTerminated(true);
                        return;
                    }
                    pathFinding.ClearObstacle();
                    StatusRobot.Navigating = false;
                    this.ResetNavigation();
                    countTentativeNavigation += 1;
                    return;
                }

                //rimuovo il primo punto , sono già nel primo putno 
                this.pointsPath.Remove(this.pointsPath.First());


                //inizio della navigazione
                while (this.pointsPath.Any())
                {
                    var correctedFromWall = await FinWallAndCorrectAngle(lidarBefore);

                    var next = this.pointsPath.First();

                    DoMovimentEndModel resMov = await DoMoviment(next);
                    if (resMov.NextMovement)
                        this.pointsPath.Remove(next);

                    //il movimento deve essere terminato , anche se è stato richiesta un nuovo ricalcolo ,
                    //deve aggiornare le posizioni anche parziali
                    await this.MovimentTerminated(resMov);

                    //end moviment
                    IEnumerable<(int, float)>? lidarAfter = await this.ReadRawLidar();
                    //for debug
                    //String jsonAfter = JsonConvert.SerializeObject(lidarAfter?.ToList());

                    //to avoid to make too many correction if not finded wall try to correct with rotation angle
                    //if (!correctedFromWall)
                    //{
                    //    if (resMov.AngleMoved != null && lidarBefore != null && lidarAfter != null)
                    //    {
                    //        await CheckAngleRotationFromLidar(lidarBefore, resMov, lidarAfter);
                    //    }
                    //    lidarBefore = await ReadRawLidar();
                    //}
                    //else {
                    //lidarBefore = lidarAfter;
                    //}


                    //for debug
                    //String jsonBefore = JsonConvert.SerializeObject(lidarBefore?.ToList());

                    //il movimento non è stato completato ed è stato richiesto il ricalcolo del path
                    if (!resMov.EndMovModel.Completed && resMov.EndMovModel.Recalculate)
                    {
                        StatusRobot.Navigating = false;
                        listLidarPointNavigation = new List<RPoint>();
                        //pathFinding.ClearObstacle();
                        //inizia un nuovo thread per raggiungere la posizione 
                        this.ResetNavigation();
                        return;
                    }
                    //il movimento è stato completato è stato richiesto un reset navigation
                    if (stopNavigation)
                    {
                        StatusRobot.Navigating = false;
                        return;
                    }

                    pathFinding.ClearObstacle();
                    lidarBefore = lidarAfter;
                    this.mapDistanceFromLidar(lidarAfter, true);

                    //if moviment are terminated without problem reset counter tentative navigation
                    countTentativeNavigation = 0;
                }
            }
            catch (ExceptionRunCommand ex)
            {
                textToSpeach.TalkAsync("Non riesco a comunicare con il robot");
                Console.WriteLine($"Impossibile comunicare con il robot {ex.Message}");
            }
            catch (Exception ex)
            {
                textToSpeach.TalkAsync("Si è verificato qualche problema , non riesco a muovermi");
                Console.WriteLine($"Si è verificato un errore {ex.Message}");
            }
            StatusRobot.Navigating = false;
            await NavigationTerminated();

        }

        private async Task CheckAngleRotationFromLidar(List<(int, float)>? lidarBefore, DoMovimentEndModel resMov, List<(int, float)>? lidarAfter)
        {
            int? angleLidar = lidarManager.FindAngleFromLidar(resMov.Point, lidarBefore, lidarAfter);
            if (angleLidar != null)
            {
                textToSpeach.TalkAsync("angolo rilevato " + angleLidar.ToString());
                Console.WriteLine("angolo rilevato " + angleLidar.ToString());
                if (resMov.AngleMoved < 270 && angleLidar > 180)
                    angleLidar = angleLidar - 180;
                var delta = Math.Abs(resMov.AngleMoved.Value) - angleLidar.Value;
                if (delta > 5 && delta < 20)
                {
                    bool turnLeft = resMov.AngleMoved < 0;
                    if (resMov.AngleMoved < 0)
                    {
                        await movement.TurnLeft(0, delta);
                    }
                    else
                    {
                        await movement.TurnRight(0, delta);
                    }
                }

            }
        }

        private async Task<bool> FinWallAndCorrectAngle(IEnumerable<(int, float)>? lidarBefore)
        {
            var pointsBefore = LidarUtility.DistanceToPoints(StatusRobot.CurrentPosition, lidarBefore);
            if (pointsBefore == null || !pointsBefore.Any())
                return false;

            var line = LidarUtility.GroupPointsByLine(pointsBefore);
            if (line == null)
                return false;

            double? lineAngle = LidarUtility.CalculateLineAngle(line.Slope);
            if (lineAngle == null)
                return false;

            await calculateDirectionRotation(lineAngle.Value);
            return true;
        }

        private async Task calculateDirectionRotation(double lineAngle)
        {
            var lineCurrentAngle =
                        StatusRobot.CurrentAngle == 0 || StatusRobot.CurrentAngle == 180 ? 0
                        : 90;
            var delta = Math.Abs(lineCurrentAngle - Math.Abs(lineAngle));
            if (delta > 45) delta = 90 - delta;
            if (delta > 40) return;
            if (Math.Abs(lineAngle) > 45)
            {
                if (lineAngle < 0)
                    await movement.TurnRight(0, (int)delta);
                else
                    await movement.TurnLeft(0, (int)delta);
            }
            else
            {
                if (lineAngle < 0)
                    await movement.TurnLeft(0, (int)delta);
                else
                    await movement.TurnRight(0, (int)delta);
            }
        }


        /// <summary>
        /// Si muove fino alla posizione da dove deve iniziare l'avvicinamento alla ricarica
        /// </summary>
        /// <returns></returns>
        public async Task NavigateToRecharge()
        {
            if (StatusRobot.NavigatingToRecharge) return;
            StatusRobot.NavigatingToRecharge = true;
            await this.ResetNavigation(RobotConfiguration.PointStartRecharge, true);
            await rechargeManager.NavigateToRecharge();
        }


        public async Task<IEnumerable<(int, float)>?> ReadObstacleFromLidar()
        {
            if (RobotConfiguration.HAVE_LIDAR)
            {
                IEnumerable<(int, float)>? dataLidar = await this.ReadRawLidar();
                this.mapDistanceFromLidar(dataLidar);
                return dataLidar;
            }
            return null;
        }

        public async Task<IEnumerable<(int, float)>?> ReadRawLidar()
        {
            if (RobotConfiguration.HAVE_LIDAR)
            {
                IEnumerable<(int, float)>? r = await lidarManager.ReadRawLidar();
                return r;
            }
            return null;
        }

        public bool[,] GetObstacleInMap()
        {
            return pathFinding.GetObstacleInMap();
        }
    }
}