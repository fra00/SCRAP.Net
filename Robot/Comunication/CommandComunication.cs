using MainRobot.Robot.Comunication.Interface;
using MainRobot.Robot.Comunication.Model;
using Serilog;

namespace MainRobot.Robot.Comunication
{
    /// <summary>
    /// Implementazione comandi del robot per il movimento e la gestione delle funzionalità base
    /// </summary>
    public class CommandComunication : ICommandComunication
    {
        private ICommandQueue enqueueComunication;
        public CommandComunication(ICommandQueue comunication)
        {
            this.enqueueComunication = comunication;   
        }

        public async Task<ComunicationCommandReceived> DisableRele1()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("05", "DisableRele1");
            return r;
        }
        public async Task<ComunicationCommandReceived> EnableRele1()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("06", "EnableRele1");
            return r;
        }

        public async Task<ComunicationCommandReceived> DisableRele2()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("07", "DisableRele2");
            return r;
        }
        public async Task<ComunicationCommandReceived> EnableRele2()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("08", "EnableRele2");
            return r;
        }
        public async Task<ComunicationCommandReceived> EnableFakeMoviment()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("09", "EnableFakeMoviment");
            StatusRobot.FakeMoviment = true;
            return r;
        }
        public async Task<ComunicationCommandReceived> Stop()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("10", "Stop");
            return r;
        }

        public async Task<ResponseSerialForward> Forward(int distance)
        {
            Log.Logger.Information("11 forward " + distance);
            ComunicationCommandReceived r = await enqueueComunication.Enqueue($"11{Configuration.SERIAL_SEPARETOR}{distance}"  , "Forward");
            try
            {
                if (r.ReceivedParam1 == Configuration.SERIAL_END_MESSAGE) return new ResponseSerialForward { Completed = true };
                var response = new ResponseSerialForward
                {
                    Completed = false,
                    DistanceObstacle = r.ReceivedParam1,
                    Angle = int.Parse(r.ReceivedParam2),
                    DistanceRunned = int.Parse(r.ReceivedParam3)
                };
                return response;
            }
            catch (Exception ex) {
                Console.WriteLine("Forward parse error");
                throw;
            }
        }

        public async Task<ComunicationCommandReceived> StartLeftMotor(int angle)
        {
            Log.Logger.Information("12 left:" + angle);
            ComunicationCommandReceived r = await enqueueComunication.Enqueue($"12{Configuration.SERIAL_SEPARETOR}{angle}", "startleft");
            return r;
        }
        public async Task<ComunicationCommandReceived> StartRightMotor(int angle)
        {
            Log.Logger.Information("13 Right:" + angle);
            ComunicationCommandReceived r = await enqueueComunication.Enqueue($"13{Configuration.SERIAL_SEPARETOR}{angle}", "startRight");
            return r;
        }

        public async Task<ComunicationCommandReceived> Backward(int distance)
        {
            Log.Logger.Information("16 Backward " + distance);
            ComunicationCommandReceived r = await enqueueComunication.Enqueue($"16{Configuration.SERIAL_SEPARETOR}{distance}", "Backward");
            return r;
        }

        public async Task<ComunicationCommandReceived> EnableMoviment()
        {
            Log.Logger.Information("21 Enable moviment");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("21", "Enable moviment");
            return r;
        }

        public async Task<ComunicationCommandReceived> DisableMoviment()
        {
            Log.Logger.Information("22 Disable moviment");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("22", "Disable moviment");
            return r;
        }

        public async Task<float?> GetLevelOfAlimentation()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("32", "GetLevelOfAlimentation");
            try
            {
                var volts = float.Parse(r.ReceivedParam1);
                return volts;
            }
            catch (Exception ex) {
                return null;
            }
        }

        public async Task<List<(float, float)>?> ReadLidar() {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("33", "ReadLidar");
            if (string.IsNullOrEmpty(r.ReceivedParam1)) return null;

            try
            {
                var floatTuple =
                    r.ReceivedParam1.Replace(".", ",")
                    .Split(Configuration.SERIAL_ARRAY_SEPARETOR)
                    .Where(c => !string.IsNullOrEmpty(c))
                    .Select(c => float.Parse(c))
                    .Chunk(2)
                    .Select(c => (c[0], c[1]))
                    .ToList();
                return floatTuple;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ReadLidar parse error");
                throw;
            }
        }

        public async Task<ComunicationCommandReceived> DisableObstacleFind()
        {
            Log.Logger.Information("14 DisableObstacleFind");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("14", "DisableObstacleFind");
            return r;
        }

        public async Task<ComunicationCommandReceived> EnableObstacleFind()
        {
            Log.Logger.Information("15 EnableObstacleFind");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("15", "EnableObstacleFind");
            return r;
        }

        public async Task<ComunicationCommandReceived> IsInRecharge()
        {
            Log.Logger.Information("17 IsInRecharge");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("17", "IsInRecharge");
            return r;
        }

        public async Task<ComunicationCommandReceived> OutInRecharge()
        {
            Log.Logger.Information("18 OutInRecharge");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("18", "OutInRecharge");
            return r;
        }

        public async Task<float> GetDistanceFrontSensor()
        {
            Log.Logger.Information("30 GetDistanceFrontSensor");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("30", "GetDistanceFrontSensor");
            try
            {
                var rs = float.Parse(r.ReceivedParam1);
                return rs;
            }
            catch (Exception ex) {
                Console.WriteLine("GetDistanceFrontSensor parse error");
                throw;
            }
            
            
        }

        public async Task<int> GetSourceOfAlimentation()
        {
            Log.Logger.Information("31 get source of alimentation");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("31", "get source of alimentation");
            try
            {
                var rs = Int32.Parse(r.ReceivedParam1);
                return rs;
            }
            catch (Exception ex) {
                return 0;
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<ComunicationCommandReceived> MoveServo(int numServo,int angle)
        {
            //0 up/down head
            //1 left/right
            Log.Logger.Information("40 move servo");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue($"40{Configuration.SERIAL_SEPARETOR}{numServo}{Configuration.SERIAL_SEPARETOR}{angle}", "get source of alimentation");
            return r;
        }



        public async Task<ComunicationCommandReceived> TestThreadSleep()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("thread-sleep", "thread-sleep");
            return r;
        }
        public async Task<ComunicationCommandReceived> TestThreadSleep1()
        {
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("thread-sleep1", "thread-sleep1");
            return r;
        }

        public async Task<ComunicationCommandReceived> ForceArduCleanStatus()
        {
            Log.Logger.Information("XX force and clean");
            ComunicationCommandReceived r = await enqueueComunication.Enqueue("XX", "ForceArduCleanStatus");
            return r;
        }
    }
}
