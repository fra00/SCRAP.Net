namespace MainRobot.Robot.Device
{
    public interface ISerial
    {
        void CloseSerial();
        void OpenSerial(Action<string> dataReceivedHandler);
        void OpenSerialByte(string serialName, int speed, Action<byte[]> dataReceivedHandler);
        void WriteLine(string message);
    }
}