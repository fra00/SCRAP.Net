namespace MainRobot
{
    /// <summary>
    /// 
    /// </summary>
    public static class Configuration
    {
        /// <summary>
        /// path of image used as map
        /// </summary>
        public static string MAP_FILE_NAME = "img//mappaMuri.png";
        /// <summary>
        /// to  speech by default is used eSpeak this is the folder when espeak is installed
        /// </summary>
        public static string PATH_ESPEAK = "C:\\Program Files (x86)\\eSpeak\\command_line\\";

        /// <summary>
        /// parameter to configure eSpeak int this case is italian voice with pitch 20
        /// </summary>
        public static string ARGUMENTS_ESPEAK = "-v it -p 20 \"[@textToSpeach]\"";

        /// <summary>
        /// serial settings : serial name
        /// </summary>
        public static string SERIAL_NAME = "COM1";
        /// <summary>
        /// serial settings : serial speed
        /// </summary>
        public static int SERIAL_SPEED = 9600;
        
        //if configured as Http use this as url to rest call
        /// <summary>
        /// http settings : url to server http arduino
        /// </summary>
        public static string HTTP_URL_COMUNICATION = "http://192.168.1.238/?cmdData=";
        
        /// <summary>
        /// TCP settings if is configured as tcp use this to configure "ip;port"
        /// </summary>
        public static string TCP_URL_COMUNICATION = "192.168.1.238;3000";

        /// <summary>
        /// separator char for command
        /// </summary>
        public static string SERIAL_SEPARETOR = "_";//"|";
        /// <summary>
        /// separator char for array elements 
        /// </summary>
        public static string SERIAL_ARRAY_SEPARETOR = ";";
        /// <summary>
        /// Start chars for message from Server (message from Server to Arduino)
        /// </summary>
        public static string SERIAL_START_MESSAGE_RPI = "RPI";
        /// <summary>
        /// Start chars for message from Server (message from Arduino to Server)
        /// </summary>
        public static string SERIAL_START_MESSAGE_ARDU = "ARDU";
        /// <summary>
        /// Chars for end message
        /// </summary>
        public static string SERIAL_END_MESSAGE = "$$\r\n"; //tcp->"$$"; //"00";


        //LLM config
        public static string HTTP_URL_BASE_LLM = "http://192.168.1.85:1234/v1/chat/";
        public static string HTTP_URL_LLM = "lmstudio-ai/gemma-2b-it-GGUF";

        //IpCamera config
        public static string CAMERA_IP_URL = "192.168.1.181";
        public static string CAMERA_IP_USR = "admin";
        public static string CAMERA_IP_PAS = "";

        //Simulate hardware 
        public static bool FAKE_HW = true;
        //simulate ardunino with remote connection (http,serial...)
        public static bool FAKE_REMOTE_ARDUINO = true;
    }
}
