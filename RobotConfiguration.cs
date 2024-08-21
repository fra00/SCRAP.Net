using MainRobot.Common;

namespace MainRobot
{
    public static class RobotConfiguration {
        public static string MESSAGGIO_AVVIO = "Ciao, mi sto avviando";
        public static string MESSAGGIO_AVVIO_COMPLETATO = "Sono pronto";
        public static string MESSAGGIO_RICARICA = "Sono scarico , vado a ricaricarmi";

        /// <summary>
        /// max width of map in cm
        /// </summary>
        public static int WIDHT_MAP = 1000;
        
        /// <summary>
        /// max heeight of map in cm
        /// </summary>
        public static int HEIGHT_MAP = 1000;
        
        /// <summary>
        /// half width of robot in cm
        /// </summary>
        public static int HALF_WIDTH_ROBOT = 15;
        
        /// <summary>
        /// half height of robot in cm
        /// </summary>
        public static int HALF_HEIGHT_ROBOT = 15;
        
        /// <summary>
        /// min step use for pathfinding
        /// </summary>
        public static int MIN_STEP_FOR_FINDPATH = 10;
        
        /// <summary>
        /// maximum distance travelled in a straight line, once this distance has been travelled the robot performs position checks
        /// </summary>
        public static int MAX_DISTANCE_FORWARD_CONSEC = 100;

        /// <summary>
        /// Each time an obstacle is detected, the weight of the point where the obstacle was detected is increased by this value
        /// </summary>
        public static int MIN_STEP_FOR_WEIGHTPATH = 70;//40

        /// <summary>
        /// used to the emulator to write the path calculation to the path.json file
        /// </summary>
        public static bool LOG_FILE_EMULATOR = true;

        /// <summary>
        /// true if robot have a lidar
        /// </summary>
        public static bool HAVE_LIDAR = true;

        /// <summary>
        /// orientation of the lidar sensor relative to the zero of the robot
        /// </summary>
        public static int ORIENTATION_LIDAR = 0;//180;
        
        /// <summary>
        /// Min distance valid for lidar
        /// </summary>
        public static int MIN_DISTANCE_LIDAR = 20;//40
        
        /// <summary>
        /// Max distance valid for lidar
        /// </summary>
        public static int MAX_DISTANCE_LIDAR = 200;
        
        /// <summary>
        /// fixed correction of calculated angle from lidar
        /// </summary>
        public static int ANGLE_CORRECTION_LIDAR = -5;
        
        /// <summary>
        /// Used for autoposition , if percentage of ovelapping point finded from lidar are greter then this percentage ,
        /// calculate autoposition
        /// </summary>
        public static int PERC_POINT_OVERLAP_AUTOPOS_LIDAR = 83;
        
        /// <summary>
        /// If the distance from teoric position and calculated position from lidar is greater than this value, invalidate autoposition
        /// </summary>
        public static int MAX_DISTANCE_AUTOPOS_LIDAR = 70;

        /// <summary>
        /// Point where the charging base is positioned
        /// </summary>
        public static RPoint PointRecharge = new RPoint(20, 420);

        /// <summary>
        /// point where the robot begins to approach the charging base
        /// </summary>
        public static RPoint PointStartRecharge = new RPoint(100,420);

        /// <summary>
        /// Angle to start recharge base
        /// </summary>
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
