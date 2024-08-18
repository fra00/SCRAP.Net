namespace MainRobot.Robot.Comunication.Model
{
    public class ComunicationCommandReceived
    {
        /// <summary>
        /// ARDU or RPI
        /// </summary>
        public string ReceiveOrigin { get; set; }
        /// <summary>
        /// Command id 
        /// </summary>
        public string ReceivedId { get; set; }
        /// <summary>
        /// Command code
        /// </summary>
        public string ReceivedCmd { get; set; }
        public string ReceivedParam1 { get; set; }
        public string ReceivedParam2 { get; set; }
        public string ReceivedParam3 { get; set; }
        public string ReceivedParam4 { get; set; }
        public bool HaveError { get; set; }

        public ComunicationCommandReceived()
        {

        }
        public ComunicationCommandReceived(string[] array)
        {
            int size = array.Length;
            if (size > 0) ReceiveOrigin = array[0];
            if (size > 1) ReceivedId = array[1];
            if (size > 2) ReceivedCmd = array[2];
            if (size > 3) ReceivedParam1 = array[3];
            if (size > 4) ReceivedParam2 = array[4];
            if (size > 5) ReceivedParam3 = array[5];
            if (size > 6) ReceivedParam4 = array[6];
        }
    }


}
