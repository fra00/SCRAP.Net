using MainRobot.Common;
using Newtonsoft.Json;

namespace MainRobot
{
    public static class StatusRobot
    {
        public static bool IsInMoviment { get; set; }
        public static bool IsInRecharge { get; set; }
        public static bool LowBattery { get; set; }
        public static bool Navigating { get; set; }
        public static bool NavigatingToRecharge { get; set; }
        public static bool FakeMoviment { get; set; }
        public static RPoint CurrentPosition { get; set; }

        public static int CurrentAngle { get; set; }



        public static void InitStatus()
        {
            var json = File.ReadAllText("status.json");
            var obj = JsonConvert.DeserializeObject<StatusFile>(json);
            if (obj == null)
            {
                obj = new StatusFile { X = 0, Y = 0, Angle = 0 };
                StatusRobot.UpdateStatus(obj);
            }
            //le coordinate devono essere multipli di 10 se non lo sono le arrotondo al valore + vicino alla decina
            int roundedX = (int)Math.Round(obj.X / 10.0) * 10;
            int roundedY = (int)Math.Round(obj.Y / 10.0) * 10;
            int roundedAngle = (int)Math.Round(obj.Angle / 10.0) * 10;

            StatusRobot.CurrentPosition = new RPoint(roundedX, roundedY);
            StatusRobot.CurrentAngle = roundedAngle;
            if (roundedX != obj.X || roundedY != obj.Y || roundedAngle != obj.Angle)
            {
                obj = new StatusFile { X = roundedX, Y = roundedY, Angle = roundedAngle };
                StatusRobot.UpdateStatus(obj);
            }
        }
        public static void UpdateStatus(StatusFile statusFile = null)
        {
            // Serializza l'array modificato in una nuova stringa json
            string newJson = JsonConvert.SerializeObject(statusFile ?? new StatusFile
            {
                X = StatusRobot.CurrentPosition.X,
                Y = StatusRobot.CurrentPosition.Y,
                Angle = StatusRobot.CurrentAngle,
                AngleStartRecharge = RobotConfiguration.AngleStartRecharge,
                PointStartRecharge = RobotConfiguration.PointStartRecharge
            });

            // Scrivi la nuova stringa json su un file di testo
            File.WriteAllText("status.json", newJson);
        }

    }

    public class StatusFile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Angle { get; set; }
        public RPoint PointStartRecharge { get; set; }
        public int AngleStartRecharge { get; set; }


    }

    public class StatusLogInfo
    {
        public List<RPoint> Path { get; set; }

        public bool EnableMoviment { get; set; }
        public bool Rele1Active { get; set; }
        public bool Rele2Active { get; set; }

    }
}