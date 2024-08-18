namespace MainRobot
{
    /// <summary>
    /// 
    /// </summary>
    public static class Configuration
    {
        public static string PATH_ESPEAK = "C:\\Program Files (x86)\\eSpeak\\command_line\\";
        
        public static string SERIAL_NAME = "COM1";
        public static int SERIAL_SPEED = 9600;
        public static string SERIAL_SEPARETOR = "_";//"|";
        public static string SERIAL_ARRAY_SEPARETOR = ";";
        public static string SERIAL_START_MESSAGE_RPI = "RPI";
        public static string SERIAL_START_MESSAGE_ARDU = "ARDU";
        public static string SERIAL_END_MESSAGE = "$$\r\n"; //tcp->"$$"; //"00";

        public static string HTTP_URL_BASE_LLM = "http://192.168.1.85:1234/v1/chat/";
        public static string HTTP_URL_LLM = "lmstudio-ai/gemma-2b-it-GGUF";

        public static string HTTP_URL_COMUNICATION = "http://192.168.1.238/?cmdData=";
        //if is configured as tcp use this to configure "ip;port"
        public static string TCP_URL_COMUNICATION = "192.168.1.238;3000";

        public static string CAMERA_IP_URL = "192.168.1.181";
        public static string CAMERA_IP_USR = "admin";
        public static string CAMERA_IP_PAS = "";

        public static bool FAKE_HW = false;
        public static bool FAKE_REMOTE_ARDUINO = false;
    }
}
