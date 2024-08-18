using MainRobot.Common;

namespace MainRobot
{
    public static class RobotConfiguration {
        public static string MESSAGGIO_AVVIO = "Ciao, mi sto avviando";
        public static string MESSAGGIO_AVVIO_COMPLETATO = "Sono pronto";
        public static string MESSAGGIO_RICARICA = "Sono scarico , vado a ricaricarmi";

        public static int WIDHT_MAP = 1000;
        public static int HEIGHT_MAP = 1000;
        public static int HALF_WIDTH_ROBOT = 15;
        public static int HALF_HEIGHT_ROBOT = 15;
        public static int MIN_STEP_FOR_FINDPATH = 10;
        public static int MAX_DISTANCE_FORWARD_CONSEC = 100;
        
        //weight increment of a point if is readed as obstacle
        public static int MIN_STEP_FOR_WEIGHTPATH = 70;//40

        public static bool LOG_FILE_EMULATOR = true;

        public static bool HAVE_LIDAR = true;

        //orientation of the lidar sensor relative to the zero of the robot
        public static int ORIENTATION_LIDAR = 0;//180;
        public static int MIN_DISTANCE_LIDAR = 20;//40
        public static int MAX_DISTANCE_LIDAR = 200;
        public static int ANGLE_CORRECTION_LIDAR = -5;
        public static int PERC_POINT_OVERLAP_AUTOPOS_LIDAR = 83;
        public static int MAX_DISTANCE_AUTOPOS_LIDAR = 70;

        public static RPoint PointRecharge = new RPoint(20, 420);
        public static RPoint PointStartRecharge = new RPoint(100,420);
        public static int AngleStartRecharge = 180;

        public static Rgba32 SxLedColorRecharge
        {
            get
            {
                return new Rgba32(200, 100, 130);
            }
        }
        public static Rgba32 SxLedTolleranceRecharge
        {
            get
            {
                return new Rgba32(55, 100, 100);
            }
        }

        public static Rgba32 FrontLedColorRecharge
        {
            get
            {
                return new Rgba32(100, 245, 100);
            }
        }
        public static Rgba32 FrontLedTolleranceRecharge
        {
            get
            {
                return new Rgba32(80, 10, 80);
            }
        }
        public static Rgba32 DxLedColorRecharge
        {
            get
            {
                return new Rgba32(245, 248, 230);
            }
        }
        public static Rgba32 DxLedTolleranceRecharge
        {
            get
            {
                return new Rgba32(10, 7, 20);
            }
        }
    }
}
