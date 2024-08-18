using MainRobot.Robot.Comunication.Model;

namespace MainRobot.Robot.Comunication.Interface
{
    public interface ICommandComunication
    {
        /// <summary>
        /// reset and clean arduino status
        /// </summary>
        /// <returns></returns>
        Task<ComunicationCommandReceived> ForceArduCleanStatus();
        /// <summary>
        /// disable rele1
        /// </summary>
        /// <returns></returns>
        Task<ComunicationCommandReceived> DisableRele1();

        Task<ComunicationCommandReceived> EnableRele1();
        Task<ComunicationCommandReceived> DisableRele2();
        Task<ComunicationCommandReceived> EnableRele2();
        Task<ComunicationCommandReceived> EnableFakeMoviment();
        Task<ComunicationCommandReceived> Stop();
        Task<ResponseSerialForward> Forward(int distance);
        Task<ComunicationCommandReceived> StartLeftMotor(int angle);
        Task<ComunicationCommandReceived> StartRightMotor(int angle);
        Task<ComunicationCommandReceived> Backward(int distance);
        Task<ComunicationCommandReceived> DisableObstacleFind();
        Task<ComunicationCommandReceived> EnableObstacleFind();
        Task<ComunicationCommandReceived> IsInRecharge();
        Task<ComunicationCommandReceived> OutInRecharge();
        Task<float> GetDistanceFrontSensor();
        Task<int> GetSourceOfAlimentation();
        Task<ComunicationCommandReceived> EnableMoviment();
        Task<ComunicationCommandReceived> DisableMoviment();
        Task<float?> GetLevelOfAlimentation();
        Task<List<(float, float)>> ReadLidar();
        Task<ComunicationCommandReceived> MoveServo(int numServo, int angle);
    }
}
