namespace Robot.Robot.Device.FakeArduino
{
    public interface IFakeArduino
    {
        void OpenSerial(Action<string> dataReceivedHandler);
        void WriteLine(string message);
    }
}