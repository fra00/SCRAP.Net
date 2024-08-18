using MainRobot.Common;
using MainRobot.Robot.Comunication.Interface;
using MainRobot.Robot.Comunication.Model;
using MainRobot.Robot.Navigation.Interface;
using MainRobot.Robot.Navigation.Model;
using Serilog;

namespace MainRobot.Robot.Navigation
{
    public class Movement : IMovement
    {
        private ICommandComunication comunication { get; set; }
        private IPathFinding pathFinding { get; set; }



        public bool RecalculatePath;
        public bool MoveToAutoPositionCheck;
        public bool AutoPositionCheck;
        public int distanceFrontSensor;
        bool IsNearToRecharge;





        public Movement(ICommandComunication comunication,
                        IPathFinding pathFinding
                        )
        {
            this.comunication = comunication;
            this.pathFinding = pathFinding;


            StatusRobot.IsInMoviment = false;
            this.IsNearToRecharge = false;
            this.RecalculatePath = false;
            this.AutoPositionCheck = false;
            this.MoveToAutoPositionCheck = false;
        }

        public async Task Stop()
        {
            try
            {
                await this.comunication.Stop();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Movement stop");
                StatusRobot.IsInMoviment = false;
                throw ex;
            }
            StatusRobot.IsInMoviment = false;
        }

        public async Task<EndMovModel> TurnRight(int angleToStop, int deltaAngle)
        {
            if (StatusRobot.IsInMoviment) return new EndMovModel { Skipped = true };
            StatusRobot.IsInMoviment = true;
            try
            {
                ComunicationCommandReceived r = await this.comunication.StartLeftMotor(deltaAngle);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Movement turn Right");
                StatusRobot.IsInMoviment = false;
                throw ex;
            }
            StatusRobot.IsInMoviment = false;
            return new EndMovModel { Completed = true };
        }

        public async Task<EndMovModel> TurnLeft(int angleToStop, int deltaAngle)
        {
            if (StatusRobot.IsInMoviment) return new EndMovModel { Skipped = true };
            StatusRobot.IsInMoviment = true;
            try
            {
                ComunicationCommandReceived r = await this.comunication.StartRightMotor(deltaAngle);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Movement turn left");
                StatusRobot.IsInMoviment = false;
                throw ex;
            }
            StatusRobot.IsInMoviment = false;
            return new EndMovModel { Completed = true };
        }

        /// <summary>
        /// Rotate robot of deltaAngle
        /// </summary>
        /// <param name="left">if true turn left</param>
        /// <param name="deltaAngle">angle to rotate</param>
        /// <returns></returns>
        public async Task<EndMovModel> Rotate(bool left, int deltaAngle)
        {
            if (StatusRobot.IsInMoviment) return new EndMovModel { Skipped = true };
            StatusRobot.IsInMoviment = true;
            try
            {
                ComunicationCommandReceived r = left ? await this.comunication.StartRightMotor(deltaAngle)
                    : await this.comunication.StartLeftMotor(deltaAngle);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Movement Rotate");
                StatusRobot.IsInMoviment = false;
                throw ex;
            }
            StatusRobot.IsInMoviment = false;
            return new EndMovModel { Completed = true };
        }

        public async Task<EndMovModel> Forward(int distance)
        {
            if (StatusRobot.IsInMoviment) return new EndMovModel { Skipped = true };
            StatusRobot.IsInMoviment = true;
            ResponseSerialForward r = null;
            try
            {
                r = await this.comunication.Forward(distance);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Movement forward");
                StatusRobot.IsInMoviment = false;
                throw ex;
            }
            var model = new EndMovModel { Completed = true };
            if (!r.Completed)
            {
                //ritorna la nuova posizione dopo aver incontrato l'ostacolo
                var nextPoint = this.ObstacleFinded(r.DistanceObstacle, r.Angle, r.DistanceRunned, 0);
                model.Point = nextPoint;
                model.Completed = false;
                model.Recalculate = true;
            }
            StatusRobot.IsInMoviment = false;
            return model;
        }

        public async Task<EndMovModel> Backward(int distance)
        {
            if (StatusRobot.IsInMoviment) return new EndMovModel { Skipped = true };
            StatusRobot.IsInMoviment = true;
            try
            {
                ComunicationCommandReceived r = await this.comunication.Backward(distance);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Movement backward");
                StatusRobot.IsInMoviment = false;
                throw ex;
            }
            StatusRobot.IsInMoviment = false;
            return new EndMovModel { Completed = true };
        }


        /// <summary>
        /// This method is called when an obstacle encountering an obstacle on the road, 
        /// Collisions are checked with bump sensor or ultrasonic sensor
        /// </summary>
        /// <param name="distance">
        /// 999;0;0 => left bump sensor pressed;
        /// 0;0;999 => right bump sensor pressed;
        /// 50;20;0 => [0] left distance from  ultrasonic sensor
        ///            [1] center distance from  ultrasonic sensor
        ///            [2] right distance from right ultrasonic sensor
        /// </param>
        /// <param name="angle"></param>
        /// <param name="distanceForward"></param>
        /// <param name="backward"></param>
        /// <returns></returns>
        public RPoint ObstacleFinded(string distance, int angle, int distanceForward, int backward)
        {
            Log.Logger.Information("Obstacle finded distance: " + distance + " angle: " + angle + "distanceForward:" + distanceForward);

            var distances = distance.Split(";").Select(int.Parse).ToArray();

            int distanceObstacle = 0;

            bool latSxObs = false;
            bool centerObs = false;
            bool latDxObs = false;

            //999 sono i baffi
            bool bSx = distances[0] == 999;
            bool bDx = distances[2] == 999;

            //se il robot ha toccato i baffi non ho una distanza da verificare nella variabile distance , ma so che la collisione
            //con i baffi è avvenuta a distanceForward
            if (bSx || bDx)
            {
                latSxObs = bSx;
                latDxObs = bDx;
                centerObs = true;
                //la distanza percorsa ritornata tiene in considerazione anche il percorso indietro e visto che 
                // ho toccato i baffi di sicuro sono tornato indietro di half_width(20 cm)
                distanceObstacle = distanceForward + RobotConfiguration.HALF_WIDTH_ROBOT;
            }
            else
            {
                latSxObs = distances[0] != 0;
                centerObs = distances[1] != 0;
                latDxObs = distances[2] != 0;
                var minDistance = distances.Where(c => c != 0).Min();
                //se l'ostacolo è nei baffi(999) la distanza è 10
                distanceObstacle = distanceForward + (minDistance <= 10 ? 10 : minDistance);
            }

            Log.Logger.Information($"currentPosition x: {StatusRobot.CurrentPosition.X} " +
                                  $"y: {StatusRobot.CurrentPosition.Y} " +
                                  $"currentAngle : {StatusRobot.CurrentAngle}");

            //calcolo la nuova posizione tenendo conto dell'eventuale spostamento indietro specificato dal parametro "backward"
            //vuo dire che il robot si è spostato indietro
            RPoint p = MathUtil.MovePointOfDistance(StatusRobot.CurrentPosition, (distanceForward - backward), (double)StatusRobot.CurrentAngle);

            var nextPoint = new RPoint();

            nextPoint.X = (int)Math.Round(Convert.ToDouble(p.X) / 10) * 10;
            nextPoint.Y = (int)Math.Round(Convert.ToDouble(p.Y) / 10) * 10;

            //valorizzo cx e cy solo per non riscrivere per ogni punto la varibile completa
            var cx = StatusRobot.CurrentPosition.X;
            var cy = StatusRobot.CurrentPosition.Y;

            //sottraggo 10 per evitare che se la distanza è 19 ad esempio , l'arrotondamento potebbe metterlo a 20
            //in quel caso il calcolo del percorso successivo potrebbe andare nuovamente contro l'ostacolo
            distanceObstacle = (int)(Math.Round(Convert.ToDouble(distanceObstacle) / 10.0) * 10);

            if (StatusRobot.CurrentAngle == 0)
            {
                if (centerObs) SetObstacle(cx + distanceObstacle, cy);
                if (latSxObs) SetObstacle(cx + distanceObstacle, cy - 10);
                if (latDxObs) SetObstacle(cx + distanceObstacle, cy + 10);
            }
            if (StatusRobot.CurrentAngle == 180)
            {
                if (centerObs) SetObstacle(cx - distanceObstacle, cy);
                if (latSxObs) SetObstacle(cx - distanceObstacle, cy + 10);
                if (latDxObs) SetObstacle(cx - distanceObstacle, cy - 10);
            }
            if (StatusRobot.CurrentAngle == 90)
            {
                if (centerObs) SetObstacle(cx, cy + distanceObstacle);
                if (latDxObs) SetObstacle(cx - 10, cy + distanceObstacle);
                if (latSxObs) SetObstacle(cx + 10, cy + distanceObstacle);
            }
            if (StatusRobot.CurrentAngle == 270)
            {
                if (centerObs) SetObstacle(cx, cy - distanceObstacle);
                if (latDxObs) SetObstacle(cx + 10, cy - distanceObstacle);
                if (latSxObs) SetObstacle(cx - 10, cy - distanceObstacle);
            }
            return nextPoint;
        }

        public void SetObstacle(int x, int y)
        {
            pathFinding.ObstacleEncountered(new RPoint(x, y));
        }

        public async Task EnableMoviment()
        {
            await comunication.EnableMoviment();
        }

        public async Task RotateXCell(int angle) {
            await comunication.MoveServo(0, angle);
        }

        public async Task RotateYCell(int angle)
        {
            await comunication.MoveServo(1, angle);
        }
    }
}
