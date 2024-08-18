namespace MainRobot.Robot.Comunication.Model
{
    public class ComunicationCommand
    {
        public int Id;
        public DateTime SendDate { get; set; }
        public string? Command { get; set; }
        public string? CommandReceived { get; set; }
        public bool SendedToArduino { get; set; }
        public bool ReceivedResponseFromArduino { get; set; }
        public bool SendedConfirmReceive { get; set; }
        public string? AliasCommand { get; set; }
        public TaskCompletionSource<ComunicationCommandReceived> Task { get; set; }
    }
}
